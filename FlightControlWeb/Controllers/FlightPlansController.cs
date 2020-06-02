using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using FlightControlWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

/*
 * This class is the controller of for the flight plans we get from internal
 * and external servers. it connects between the html client to the server
 * it contains the flight context (the DB) and updates
 * it according to the changes that occur in the program.
 */
namespace FlightControlWeb.Controllers
{
    [Route("api/FlightPlan")]
    [ApiController]
    public class FlightPlansController : ControllerBase
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly FlightContext _flightContext;
        private readonly IFlightManager _flightManager;
        private readonly IFlightPlanManager _flightPlanManager;

        public FlightPlansController(FlightContext flightContext, IFlightManager flightManager, 
            IFlightPlanManager flightPlanManager, IHttpClientFactory clientFactory)
        {
            _flightContext = flightContext;
            _flightPlanManager = flightPlanManager;
            _flightManager = flightManager;
            _clientFactory = clientFactory;
        }

        /*
         * This method creates a copy of an existing flight plan and returns it as Data 
         * so that the user who asked for it would get it in a valid format.
         */
        private async Task<ActionResult<FlightPlanData>> 
            BuildMatchingFlightPlan(string id, FlightPlan flightPlan)
        {
            var matchingFlight = await _flightContext.FlightItems.Where(x => x.FlightId == flightPlan.FlightId)
                .FirstOrDefaultAsync();
            var matchingInitialLocation = await _flightContext.InitialLocationItems
                .Where(x => x.FlightPlanId == flightPlan.Id).FirstOrDefaultAsync();
            IEnumerable<Segment> matchingSegments =
                await _flightContext.SegmentItems.Where
                    (x => x.FlightPlanId == flightPlan.Id).ToListAsync();
            var flightPlanData = new FlightPlanData
            {
                Passengers = flightPlan.Passengers,
                CompanyName = flightPlan.CompanyName,
                InitialLocation = matchingInitialLocation,
                Segments = matchingSegments
            };

            return flightPlanData;
        }

        /*
         * Getting flights from external servers using a http client
         */
        private async Task<ActionResult<dynamic>> GetExternalFlightPlan(string id, Flight flight)
        {
            var _apiUrl = flight.OriginServer + "/api/FlightPlan/" + flight.FlightId;
            var _baseAddress = flight.OriginServer;
            using (var client = _clientFactory.CreateClient())
            {
                client.BaseAddress = new Uri(_baseAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add
                    (new MediaTypeWithQualityHeaderValue("application/json"));
                try
                {
                    var result = await client.GetStringAsync(_apiUrl);
                    return result;
                }
                catch (Exception e)
                {
                    Console.WriteLine();
                    return null;
                }
                
                return NotFound();
            }
        }
        /*
         * Once the user types or ask for the GET with the flight id, this method finds
         * the specific flight we look for in our DB. if it exists, we return it,
         * otherwise we return NOT FOUND.
         */
        // GET: api/FlightPlans/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FlightPlanData>> GetFlightPlan(string id)
        {
            var flight = await _flightContext.ExternalFlightItems.Where
                (x => x.FlightId == id).FirstOrDefaultAsync();

            if (flight.IsExternal)
            {
                return await GetExternalFlightPlan(id, flight);
            }
            var flightPlan = await _flightContext.FlightPlanItems.Where
                (x => x.FlightId == id).FirstOrDefaultAsync();

            if (flightPlan == null) return NotFound();

            return await BuildMatchingFlightPlan(id, flightPlan);
        }

        /*
         * This method  gets the info as a raw list of files external server or a client
         * and parse it into a string.
         */
        private StringBuilder BuildString(List<IFormFile> files)
        {
            var result = new StringBuilder();
            foreach (var file in files)
                using (var reader = new StreamReader(file.OpenReadStream()))
                {
                    while (reader.Peek() >= 0)
                        result.AppendLine(reader.ReadLine());
                }

            return result;
        }

        /*
         * This method converts the result to a string and returns it as a JSON type. 
         */
        private dynamic ConvertAndDeserialize(StringBuilder result)
        {
            var input = result.ToString();
            dynamic bodyObj = JsonConvert.DeserializeObject(input);
            return bodyObj;
        }

        /*
         * This method adds new initial locations we get from each flight in our DBs
         * and return it.
         */
        private InitialLocation AddInitialLocation(FlightPlan newFlightPlan, double longitude,
            double latitude, DateTime dateTime)
        {
            var newInitialLocation = new InitialLocation
            {
                Latitude = latitude,
                Longitude = longitude,
                DateTime = dateTime,
                FlightPlanId = newFlightPlan.Id
            };
            _flightContext.InitialLocationItems.Add(newInitialLocation);

            return newInitialLocation;
        }

        /*
         * This method adds new segments we get from each flight in our DBs
         * and return the last segment's end time.
         */
        private DateTime AddSegments(DateTime dateTime, dynamic segmentsObj,
            FlightPlan newFlightPlan)
        {
            var start = dateTime;
            var end = start;
            foreach (var segment in segmentsObj)
            {
                double timespan = segment["timespan_seconds"];
                end = start.AddSeconds(timespan);
                var newSegment = new Segment
                {
                    Longitude = segment["longitude"],
                    Latitude = segment["latitude"],
                    TimeSpanSeconds = segment["timespan_seconds"],
                    FlightPlanId = newFlightPlan.Id,
                    StartTime = start,
                    EndTime = end
                };
                start = end;
                _flightContext.SegmentItems.Add(newSegment);
            }

            return end;
        }

        /*
         * This method updates all the related DBs with the new objects we just created
         * from the data we received. we return conflict if the object is already exist in our DBs
         * otherwise, we show the new flight data.
         */
        private async Task<IActionResult> UpdateDb(Flight newFlight, FlightPlan newFlightPlan,
            InitialLocation newInitialLocation)
        {
            try
            {
                await _flightContext.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                if (FlightExists(newFlight.Id) || FlightPlanExists(newFlightPlan.Id) ||
                    InitialLocationExists(newInitialLocation.Id))
                    return Conflict();

                Debug.WriteLine(e.Message);
                throw;
            }

            return CreatedAtAction
                ("GetFlightPlan", new {id = newFlightPlan.FlightId}, newFlightPlan);
        }

        /*
         * This method gets an object (JSON) and parse it.
         * for each flight we create a new flight, flight plan, initial location and segments objects
         * and fill each related part.
         * when we done parsing and creating each object we add it to our DBs. 
         */
        private async Task<IActionResult> AddObjects(dynamic bodyObj)
        {
            int passengers = bodyObj["passengers"];
            string companyName = bodyObj["company_name"];
            double longitude = bodyObj["initial_location"]["longitude"];
            double latitude = bodyObj["initial_location"]["latitude"];
            DateTime dateTime = bodyObj["initial_location"]["date_time"];
            var segmentsObj = bodyObj["segments"];
            
            var newFlight = _flightManager.AddFlight();
            await _flightContext.FlightItems.AddAsync(newFlight);

            var newFlightPlan = _flightPlanManager.AddFlightPlan
                (newFlight, passengers, companyName);
            await _flightContext.FlightPlanItems.AddAsync(newFlightPlan);
            
            var newInitialLocation = AddInitialLocation
                (newFlightPlan, longitude, latitude, dateTime);

            DateTime end = AddSegments(dateTime, segmentsObj, newFlightPlan);

            newFlightPlan.EndTime = end;

            return await UpdateDb(newFlight, newFlightPlan, newInitialLocation);
        }

        // POST: api/FlightPlans
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.

        /*
         * This method is a POST implementation. this way we add new flights to our DB.
         * it gets the info as a list of raw JSON file from an external server or a client and parse it.
         * for each flight we create a new flight plan object and fill each related part.
         * when we done parsing and creating each flight plan object we add it to our DB. 
         */
        [HttpPost]
        public async Task<IActionResult> PostFlightPlan(List<IFormFile> files)
        {
            var result = BuildString(files);
            var bodyObj = ConvertAndDeserialize(result);

            try
            {
                AddObjects(bodyObj);
            }
            catch (Exception)
            {
                throw new ArgumentException("failed to post flight plan");
            }

            return StatusCode(201);
        }

        /*
         * This method gets a flight ID as an argument and returns true if this flight plan
         * exists in our DB. otherwise it returns false.
         */
        private bool FlightPlanExists(int id)
        {
            return _flightContext.FlightPlanItems.Any(e => e.Id == id);
        }

        /*
         * This method gets a flight ID as an argument and returns true if this flight
         * exists in our DB. otherwise it returns false.
         */
        private bool FlightExists(int id)
        {
            return _flightContext.FlightItems.Any(e => e.Id == id);
        }

        /*
         * This method gets a flight ID as an argument and returns true if its initial
         * locations exist in our DB. otherwise it returns false.
         */
        private bool InitialLocationExists(int id)
        {
            return _flightContext.InitialLocationItems.Any(e => e.Id == id);
        }
    }
}