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

        // GET: api/FlightPlans/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FlightPlan>> GetFlightPlan(string id)
        {
            IEnumerable<FlightPlan> allFlightPlans = await _flightContext.FlightPlanItems.ToListAsync();

            var flightPlan = await _flightContext.FlightPlanItems.Where(x => x.FlightId == id).FirstOrDefaultAsync();

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

            string input = body.ToString();
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

        private bool FlightPlanExists(string id)
        {
            return _flightContext.FlightPlanItems.Any(e => e.FlightId == id);
        }
    }
}
