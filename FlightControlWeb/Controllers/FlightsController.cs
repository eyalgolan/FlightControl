using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using FlightControlWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

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

        /* 
        * This method gets time and finds all the flight plans that their related flight
        * occurs before starts before the given time
        */
        private async Task<IEnumerable<FlightPlan>> FindPlanStartingBefore(DateTime relative_to)
        {
            IEnumerable<InitialLocation> relaventInitials =
                await _flightContext.InitialLocationItems.Where(x => x.DateTime <= relative_to).ToListAsync();
            var relaventPlans = Enumerable.Empty<FlightPlan>();
            foreach (var initial in relaventInitials)
            {
                var initialFlightPlanId = initial.FlightPlanId;
                var relevantPlan = await _flightContext.FlightPlanItems.FindAsync(initialFlightPlanId);
                if (relevantPlan != null) relaventPlans = relaventPlans.Append(relevantPlan);
            }

            return relaventPlans;
        }

        /* 
        * This method find the specific segment that the flight is in at given time, and updates it
        * live so we can see in each and every moment on the HTTP view where the flight is at
        */
        private static void FindAndUpdateLocation(DateTime relative_to, SortedDictionary<int, Segment> planSegmentDict,
            double secondsInFlight, InitialLocation currentInitial, FlightData relevantFlightData, FlightPlan plan)
        {
            foreach (var k in planSegmentDict)
                // if the seconds that passed since the beginning of the flight are greater 
                // than this segment's duration
                if (secondsInFlight > k.Value.TimeSpanSeconds)
                {
                    secondsInFlight -= k.Value.TimeSpanSeconds;
                }
                else
                {
                    var secondsInSegment = k.Value.TimeSpanSeconds - (int) secondsInFlight;
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

                    var delta = 1 - secondsInSegment / (double) k.Value.TimeSpanSeconds;
                    relevantFlightData.Latitude =
                        lastLatitude + delta * (k.Value.Latitude - lastLatitude);
                    relevantFlightData.Longitude =
                        lastLongitude + delta * (k.Value.Longitude - lastLongitude);
                    planSegmentDict.Clear();
                    break;
                }

            relevantFlightData.Passengers = plan.Passengers;
            relevantFlightData.CompanyName = plan.CompanyName;
            relevantFlightData.CurrDateTime = relative_to;
        }

        /* todo
        * This method updates the data of a flight (especially location) and add it to
        * the relevant flights list.
        */
        private async Task<IEnumerable<FlightData>> UpdateAndAddFlight(DateTime relative_to,
            InitialLocation currentInitial, FlightPlan currentPlan, FlightData relevantFlightData, FlightPlan plan,
            IEnumerable<FlightData> relevantFlights)
        {
            var secondsInFlight = (relative_to - currentInitial.DateTime).TotalSeconds;

            IEnumerable<Segment> planSegments = await _flightContext.SegmentItems
                .Where(x => x.FlightPlanId == currentPlan.Id).ToListAsync();
            var planSegmentDict = new SortedDictionary<int, Segment>();
            var index = 0;
            foreach (var planSegment in planSegments)
            {
                planSegmentDict.Add(index, planSegment);
                index++;
            }

            FindAndUpdateLocation(relative_to, planSegmentDict, secondsInFlight, currentInitial, relevantFlightData,
                plan);

            relevantFlights = relevantFlights.Append(relevantFlightData);

            return relevantFlights;
        }


        /*
         * This method find relevant internal flights.
         * it gets a relevant flight plans and time, and if the flights occurs after
         * the given time it creates a flight data object and adds it to a list.
         * in the it returns all the relevant data for these flights.
         */
        private async Task<IEnumerable<FlightData>> FindRelevantInternalFlights(IEnumerable<FlightPlan> relevantPlans,
            DateTime relative_to)
        {
            IEnumerable<FlightData> relevantFlights = new List<FlightData>();
            foreach (var plan in relevantPlans)
                if (plan.EndTime >= relative_to)
                {
                    var relevantFlight = await _flightContext.FlightItems.Where(x => x.FlightId == plan.FlightId)
                        .FirstOrDefaultAsync();
                    var relevantFlightData = new FlightData
                    {
                        FlightID = relevantFlight.FlightId
                    };

                    var currentInitial = await _flightContext.InitialLocationItems
                        .Where(x => x.FlightPlanId == plan.Id).FirstOrDefaultAsync();
                    relevantFlights = await UpdateAndAddFlight(relative_to, currentInitial, plan, relevantFlightData,
                        plan,
                        relevantFlights);
                }

            return relevantFlights;
        }

        private async Task<IEnumerable<FlightData>> AddExternalFlights(IEnumerable<FlightData> externalFlights,
            string relativeTo)
        {
            //todo need to check this works
            var servers = await _flightContext.ServerItems.ToListAsync();
            foreach (var server in servers)
            {
                var _apiUrl = server.ServerUrl + "/api/Flights?relative_to=" + "2020-05-24T14:56:24:315Z";
                var _baseAddress = server.ServerUrl;
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_baseAddress);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    
                    var result = await client.GetAsync(_apiUrl);

                    if (result.IsSuccessStatusCode)
                    {
                        var response = await result.Content.ReadAsStreamAsync();
                        var input = response.ToString();
                        IEnumerable<FlightData> responseObj = JsonConvert.DeserializeObject<IEnumerable<FlightData>>(input);

                        foreach (var flightData in responseObj)
                        {
                            var flight = new Flight()
                            {
                                FlightId = flightData.FlightID,
                                OriginServer = server.ServerUrl,
                                IsExternal = true
                            };

                            await _flightContext.ExternalFlightItems.AddAsync(flight);
                            externalFlights = externalFlights.Append(flightData);
                        }
                    }
                }
            }

            return externalFlights;
        }

        /*
        * Our Implementation to HTTP GET method.
        * Once the user types or ask for the GET, this method finds all the flights that
        * in our DB. then, we put it in a list of flights and return it
        */
        // GET: api/Flights
        [HttpGet]
        public async Task<IEnumerable<FlightData>> GetFlights([FromQuery] string relative_to)
        {
            DateTime relativeTo = DateTime.Parse(relative_to);
            relativeTo = relativeTo.ToUniversalTime();
            
            var relevantPlans = await FindPlanStartingBefore(relativeTo);

            IEnumerable<FlightData> internalFlights = new List<FlightData>();
            IEnumerable<FlightData> externalFlights = new List<FlightData>();

            internalFlights = await FindRelevantInternalFlights(relevantPlans, relativeTo);

            var requestValue = Request.QueryString.Value;
            if (requestValue.Contains("sync_all"))
                externalFlights = await AddExternalFlights(externalFlights, relative_to);

            internalFlights = internalFlights.Concat(externalFlights);

            return internalFlights;
        }


        /*
         * Our implementation for the HTTP DELETE method.
         * This method gets a flight ID as an argument and removes its related flight
         * and its flight plan from our DB.
         * if the Flight does not exist we return not found error.
         */
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

        /*
         * This method gets a flight ID as an argument and returns true if this flight
         * exists in our DB. otherwise it returns false.
         */
        private bool FlightExists(string id)
        {
            return _flightContext.FlightItems.Any(e => e.FlightId == id);
        }
    }
}