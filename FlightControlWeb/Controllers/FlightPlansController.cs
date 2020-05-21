using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FlightControlWeb.Models;
using Microsoft.AspNetCore.Routing.Constraints;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;
using Microsoft.AspNetCore.Http;

namespace FlightControlWeb.Controllers
{
    /*
     * This class is the controller of for the flight plans we get from internal
     * and external servers. it connects between the html client to the server
     * it contains the flight context (the DB) and updates
     * it according to the changes that occur in the program.
     */
    [Route("api/FlightPlan")]
    [ApiController]
    public class FlightPlansController : ControllerBase
    {
        private readonly IDataContext _flightContext;
        private readonly IFlightPlanManager _flightPlanManager;
        private readonly IFlightManager _flightManager;
        public FlightPlansController(FlightContext flightContext)
        {
            _flightContext = flightContext;
            _flightPlanManager = new FlightPlanManager();
            _flightManager = new FlightManager();
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
            IEnumerable<FlightPlan> allFlightPlans = await _flightContext.FlightPlanItems.ToListAsync();

            var flightPlan = await _flightContext.FlightPlanItems.Where(x => x.FlightId == id).FirstOrDefaultAsync();


            if (flightPlan == null)
            {
                return NotFound();
            }

            var matchingFlight = await _flightContext.FlightItems.Where(x => x.FlightId == flightPlan.FlightId)
                .FirstOrDefaultAsync();
            var matchingInitialLocation = await _flightContext.InitialLocationItems
                .Where(x => x.FlightPlanId == flightPlan.Id).FirstOrDefaultAsync();
            IEnumerable<Segment> matchingSegments =
                await _flightContext.SegmentItems.Where(x => x.FlightPlanId == flightPlan.Id).ToListAsync();
            var flightPlanData = new FlightPlanData();
            flightPlanData.passengers = flightPlan.Passengers;
            flightPlanData.company_name = flightPlan.CompanyName;
            flightPlanData.initial_location = matchingInitialLocation;
            flightPlanData.segments = matchingSegments;

            return flightPlanData;
        }

        // POST: api/FlightPlans
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.

        /*
         * This method is a POST implementation. this way we add new flights to our DB.
         * it gets the info as JSON file from an external server or a client and parse it.
         * for each flight we create a new flight plan object and fill each related part.
         * when we done parsing and creating each flight plan object we add it to our DB. 
         */
        [HttpPost]
        public async Task<IActionResult> PostFlightPlan(List<IFormFile> files)
        {

            var result = new System.Text.StringBuilder();
            foreach (var file in files)
            {
                using (var reader = new StreamReader(file.OpenReadStream()))
                {
                    while (reader.Peek() >= 0)
                        result.AppendLine(reader.ReadLine());
                }
            }


            string input = result.ToString();
            dynamic bodyObj = JsonConvert.DeserializeObject(input);

            int passengers = bodyObj["passengers"];
            string companyName = bodyObj["company_name"];
            double longitude = bodyObj["initial_location"]["longitude"];
            double latitude = bodyObj["initial_location"]["latitude"];
            DateTime dateTime = bodyObj["initial_location"]["date_time"];

            Flight newFlight = new Flight();
            _flightManager.CreateId(newFlight);
            _flightContext.FlightItems.Add(newFlight);

            FlightPlan newFlightPlan = new FlightPlan
            {
                FlightId = newFlight.FlightId,
                IsExternal = false
            };
            //_flightPlanManager.CreateId(newFlightPlan);
            newFlightPlan.Passengers = passengers;
            newFlightPlan.CompanyName = companyName;
            _flightContext.FlightPlanItems.Add(newFlightPlan);

            InitialLocation newInitialLocation = new InitialLocation
            {
                //_flightPlanManager.CreateId(newInitialLocation);
                Latitude = latitude,
                Longitude = longitude,
                DateTime = dateTime,
                FlightPlanId = newFlightPlan.Id
            };
            _flightContext.InitialLocationItems.Add(newInitialLocation);

            dynamic segmentsObj = bodyObj["segments"];
            DateTime start = dateTime;
            DateTime end = start;
            foreach (var segment in segmentsObj)
            {
                double timespan = segment["timespan_seconds"];
                end = start.AddSeconds(timespan);
                Segment newSegment = new Segment
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

            newFlightPlan.EndTime = end;
            await _flightContext.SaveChangesAsync();


            try
            {
                await _flightContext.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (FlightPlanExists(newFlightPlan.FlightId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }
            

            return CreatedAtAction("GetFlightPlan", new { id = newFlightPlan.FlightId }, newFlightPlan);
    
    }

        /*
         * This method gets a flight ID as an argument and returns true if this flight
         * exists in our DB. otherwise it return false.
         */
        private bool FlightPlanExists(string id)
        {
            return _flightContext.FlightPlanItems.Any(e => e.FlightId == id);
        }
    }
}
