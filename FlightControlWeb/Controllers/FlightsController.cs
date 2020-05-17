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


        // GET: api/Flights
        [HttpGet]
        public async Task<IEnumerable<FlightData>> GetFlights([FromQuery] DateTime relative_to, [FromQuery] bool? sync_all = false)
        {
            relative_to = relative_to.ToUniversalTime();
            //bool syncAll = sync_all.HasValue ? sync_all.Value : false;

            IEnumerable<InitialLocation> relaventInitials =
                await _flightContext.InitialLocationItems.Where(x => x.DateTime <= relative_to).ToListAsync();
            IEnumerable<FlightPlan> relaventPlans = Enumerable.Empty<FlightPlan>();
            foreach (var initial in relaventInitials)
            {
                int segmentFlightPlanId = initial.FlightPlanId;
                var relaventPlan = await _flightContext.FlightPlanItems.FindAsync(segmentFlightPlanId);
                if (relaventPlan != null) relaventPlans = relaventPlans.Append(relaventPlan);
            }

            IEnumerable<FlightData> relaventFlights = new List<FlightData>();
            foreach (var plan in relaventPlans)
                if (plan.EndTime >= relative_to)
                {
                    var relaventFlight = await _flightContext.FlightItems.Where(x => x.FlightId == plan.FlightId).FirstOrDefaultAsync();
                    var relaventFlightData = new FlightData();
                    relaventFlightData.flight_id = relaventFlight.FlightId;
                    
                    if (relaventFlight != null)
                    {
                        var currentPlan = await _flightContext.FlightPlanItems.Where(x => x.FlightId == relaventFlight.FlightId).FirstOrDefaultAsync();
                        var currentInitial = await _flightContext.InitialLocationItems
                            .Where(x => x.FlightPlanId == currentPlan.Id).FirstOrDefaultAsync();
                        // (the universal time) - (this fligt start) = how much seconds passed since the flight started
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
                                relaventFlightData.latitude = lastLatitude + (delta * (k.Value.Latitude - lastLatitude));
                                relaventFlightData.longitude = lastLongitude + (delta * (k.Value.Longitude - lastLongitude));
                                planSegmentDict.Clear();
                                break;
                            }
                        }


                        //relaventFlight.latitude = _flightManager.GetFlightLatitude(relaventFlight);
                        //relaventFlight.longitude = _flightManager.GetFlightLongitude(relaventFlight);
                        relaventFlightData.passengers = plan.Passengers;
                        relaventFlightData.company_name = plan.CompanyName;
                        relaventFlightData.date_time = relative_to;
                        relaventFlights = relaventFlights.Append(relaventFlightData);
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
                                relaventFlights.Append(flight);
                            }
                        }
                    }
                }
            }

            return relaventFlights;
        }



        // DELETE: api/Flights/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Flight>> DeleteFlight(string id)
        {
            //var flight = await _flightContext.FlightItems.FindAsync(id);
            //await db.Foos.Where(x => x.UserId == userId).ToListAsync();
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