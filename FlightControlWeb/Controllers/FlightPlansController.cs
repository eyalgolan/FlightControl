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

namespace FlightControlWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightPlansController : ControllerBase
    {
        private readonly FlightContext _flightContext;
        private readonly IFlightPlanManager _flightPlanManager;
        private readonly IFlightManager _flightManager;
        public FlightPlansController(FlightContext flightContext)
        {
            _flightContext = flightContext;
            _flightPlanManager = new FlightPlanManager();
            _flightManager = new FlightManager();
        }

        // GET: api/FlightPlans/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FlightPlan>> GetFlightPlan(string id)
        {
            IEnumerable<FlightPlan> allFlightPlans = await _flightContext.FlightPlanItems.ToListAsync();

            var flightPlan = _flightPlanManager.GetFlightPlanByFlightId(allFlightPlans, id);

            if (flightPlan == null)
            {
                return NotFound();
            }

            return flightPlan;
        }

        // POST: api/FlightPlans
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<FlightPlan>> PostFlightPlan([FromBody] JsonElement body)
        {

            int passengers = body.GetProperty("passengers").GetInt32();
            string companyName = body.GetProperty("company_name").GetString();
            double longitude = body.GetProperty("initial_location").GetProperty("longitude").GetDouble();
            double latitude = body.GetProperty("initial_location").GetProperty("latitude").GetDouble();
            //DateTime dateTime = body.GetProperty("date_time").GetDateTime();

            Flight newFlight = new Flight();
            _flightManager.CreateId(newFlight);
            newFlight.IsExternal = false;
            _flightContext.FlightItems.Add(newFlight);

            FlightPlan newFlightPlan = new FlightPlan();
            newFlightPlan.FlightId = newFlight.FlightId;
            _flightPlanManager.CreateId(newFlightPlan);
            newFlightPlan.Passengers = passengers;
            newFlightPlan.CompanyName = companyName;
            _flightContext.FlightPlanItems.Add(newFlightPlan);

            InitialLocation newInitialLocation = new InitialLocation();
            _flightPlanManager.CreateId(newInitialLocation);
            newInitialLocation.Latitude = latitude;
            newInitialLocation.Longitude = longitude;
            //newInitialLocation.DateTime = dateTime;
            newInitialLocation.FlightPlanId = newFlightPlan.Id;
            _flightContext.InitialLocationItems.Add(newInitialLocation);

            JsonElement segments = body.GetProperty("segments");
            string segmentString = segments.ToString();
            dynamic segmentsObj = JsonConvert.DeserializeObject(segmentString);
            foreach (var segment in segmentsObj)
            {
                Segment newSegment = new Segment();
                newSegment.Longitude = segment["longitude"];
                newSegment.Latitude = segment["latitude"];
                newSegment.TimeSpanSeconds = segment["timespan_seconds"];
                newSegment.FlightPlanId = newFlightPlan.Id;
                _flightContext.SegmentItems.Add(newSegment);
            }


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

        private bool FlightPlanExists(string id)
        {
            return _flightContext.FlightPlanItems.Any(e => e.FlightId == id);
        }
    }
}
