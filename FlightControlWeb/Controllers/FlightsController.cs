using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using FlightControlWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FlightControlWeb.Controllers
{
    [Route("api/Flights")]
    [ApiController]
    public class FlightsController : ControllerBase
    {
        private readonly FlightContext _flightContext;
        private readonly IFlightManager _flightManager;

        public FlightsController(FlightContext flightContext)
        {
            _flightContext = flightContext;
            _flightManager = new FlightManager();
        }

        /*
        // GET: api/Flights/
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Segment>>> GetTodoItems()
        {
            return await _flightContext.SegmentItems.ToListAsync();
        }
        */


        private async Task<IEnumerable<FlightPlan>> FindPlanStartingBefore(DateTime relative_to)
        {
            IEnumerable<InitialLocation> relaventInitials =
                await _flightContext.InitialLocationItems.Where(x => x.DateTime <= relative_to).ToListAsync();
            IEnumerable<FlightPlan> relaventPlans = Enumerable.Empty<FlightPlan>();
            foreach (var initial in relaventInitials)
            {
                int initialFlightPlanId = initial.FlightPlanId;
                var relaventPlan = await _flightContext.FlightPlanItems.FindAsync(initialFlightPlanId);
                if (relaventPlan != null) relaventPlans = relaventPlans.Append(relaventPlan);
            }

            return relaventPlans;
        }

        public async Task<IEnumerable<Flight>> FindRelevantFlights()
        {

        }

        // GET: api/Flights
        [HttpGet]
        public async Task<IEnumerable<FlightData>> GetFlights([FromQuery] DateTime relative_to, [FromQuery] bool? sync_all = false)
        {
            relative_to = relative_to.ToUniversalTime();

            IEnumerable<FlightPlan> relevantPlans = await FindPlanStartingBefore(relative_to);

            IEnumerable<FlightData> relevantFlights = new List<FlightData>();
            foreach (var plan in relevantPlans)
                if (plan.EndTime >= relative_to)
                {
                    var relevantFlight = await _flightContext.FlightItems.Where(x => x.FlightId == plan.FlightId).FirstOrDefaultAsync();
                    var relevantFlightData = new FlightData
                    {
                        flight_id = relevantFlight.FlightId
                    };

                    if (relevantFlight != null)
                    {
                        var currentPlan = await _flightContext.FlightPlanItems.Where(x => x.FlightId == relevantFlight.FlightId).FirstOrDefaultAsync();
                        var currentInitial = await _flightContext.InitialLocationItems
                            .Where(x => x.FlightPlanId == currentPlan.Id).FirstOrDefaultAsync();
                        
                        double secondsInFlight = (relative_to - currentInitial.DateTime).TotalSeconds;
                        
                        IEnumerable<Segment> planSegments = await _flightContext.SegmentItems
                            .Where(x => x.FlightPlanId == currentPlan.Id).ToListAsync();
                        SortedDictionary<int, Segment> planSegmentDict = new SortedDictionary<int, Segment>();
                        int index = 0;
                        foreach (var planSegment in planSegments)
                        {
                            planSegmentDict.Add(index, planSegment);
                            index++;
                        }

                        foreach (KeyValuePair<int, Segment> k in planSegmentDict)
                        {
                            Console.WriteLine("key {0}", k.Key);
                        }
                        foreach (KeyValuePair<int, Segment> k in planSegmentDict)
                        {
                            // if the seconds that passed since the beginning of the flight are greater 
                            // than this segment's duration
                            if (secondsInFlight > k.Value.TimeSpanSeconds)
                            {
                                secondsInFlight -= k.Value.TimeSpanSeconds;
                            }
                            else
                            {
                                int secondsInSegment = k.Value.TimeSpanSeconds - (int) secondsInFlight;
                                double lastLatitude;
                                double lastLongitude;
                                if (k.Key == 0)
                                {
                                    lastLongitude = currentInitial.Longitude;
                                    lastLatitude = currentInitial.Latitude;
                                }
                                else
                                {
                                    var previousSegment = planSegmentDict[k.Key - 1];
                                    lastLongitude = previousSegment.Longitude;
                                    lastLatitude = previousSegment.Latitude;
                                }

                                double delta = (secondsInSegment / (double) k.Value.TimeSpanSeconds);
                                relevantFlightData.latitude = lastLatitude + (delta * (k.Value.Latitude - lastLatitude));
                                relevantFlightData.longitude = lastLongitude + (delta * (k.Value.Longitude - lastLongitude));
                                planSegmentDict.Clear();
                                break;
                            }
                        }


                        relevantFlightData.passengers = plan.Passengers;
                        relevantFlightData.company_name = plan.CompanyName;
                        relevantFlightData.date_time = relative_to;
                        relevantFlights = relevantFlights.Append(relevantFlightData);
                    }
                }

            //todo need to check this works
            if (sync_all == null)
            {
                IEnumerable<Server> servers = _flightContext.Set<Server>();
                foreach (var server in servers)
                {
                    string _apiUrl = server.ServerURL + "/api/flights?relative_to=" + relative_to;
                    string _baseAddress = server.ServerURL;
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(_baseAddress);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        var result = await client.GetAsync(_apiUrl);

                        if (result.IsSuccessStatusCode)
                        {
                            IEnumerable<FlightData> response = result.Content.ReadAsAsync<IEnumerable<FlightData>>().Result;
                            foreach (var flight in response)
                            {
                                relevantFlights.Append(flight);
                            }
                        }
                    }
                }
            }

            return relevantFlights;
        }



        // DELETE: api/Flights/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Flight>> DeleteFlight(string id)
        {
            var flight = await _flightContext.FlightItems.Where(x => x.FlightId == id).FirstOrDefaultAsync();
            if (flight == null) return NotFound();

            var flightPlan = await _flightContext.FlightPlanItems.Where(x => x.FlightId == id).FirstOrDefaultAsync();
            _flightContext.FlightPlanItems.Remove(flightPlan);
            _flightContext.FlightItems.Remove(flight);
            await _flightContext.SaveChangesAsync();

            return flight;
        }

        private bool FlightExists(string id)
        {
            return _flightContext.FlightItems.Any(e => e.FlightId == id);
        }
    }
}