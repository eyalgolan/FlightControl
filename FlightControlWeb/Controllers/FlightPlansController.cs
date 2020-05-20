using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
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

        private StringBuilder BuildString(List<IFormFile> files)
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

            return result;
        }

        private dynamic ConvertAndDeserialize(StringBuilder result)
        {
            string input = result.ToString();
            dynamic bodyObj = JsonConvert.DeserializeObject(input);
            return bodyObj;
        }

        private Flight AddFlight()
        {
            Flight newFlight = new Flight();
            _flightManager.CreateId(newFlight);
            _flightContext.FlightItems.Add(newFlight);

            return newFlight;
        }

        private FlightPlan AddFlightPlan(Flight newFlight, int passengers, string companyName)
        {
            FlightPlan newFlightPlan = new FlightPlan
            {
                FlightId = newFlight.FlightId,
                IsExternal = false
            };

            newFlightPlan.Passengers = passengers;
            newFlightPlan.CompanyName = companyName;
            _flightContext.FlightPlanItems.Add(newFlightPlan);

            return newFlightPlan;
        }

        private InitialLocation AddInitialLocation(FlightPlan newFlightPlan, double longitude, double latitude, DateTime dateTime)
        {
            InitialLocation newInitialLocation = new InitialLocation
            {
                Latitude = latitude,
                Longitude = longitude,
                DateTime = dateTime,
                FlightPlanId = newFlightPlan.Id
            };
            _flightContext.InitialLocationItems.Add(newInitialLocation);

            return newInitialLocation;
        }

        private DateTime AddSegments(DateTime dateTime, dynamic segmentsObj, FlightPlan newFlightPlan)
        {
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

            return end;
        }

        private async Task<IActionResult> UpdateDb(Flight newFlight, FlightPlan newFlightPlan, InitialLocation newInitialLocation)
        {
            try
            {
                await _flightContext.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                if (FlightExists(newFlight.Id) || FlightPlanExists(newFlightPlan.Id) || InitialLocationExists(newInitialLocation.Id))
                {
                    return Conflict();
                }
                else
                {
                    Debug.WriteLine(e.Message);
                    throw;
                }
            }

            return CreatedAtAction("GetFlightPlan", new { id = newFlightPlan.FlightId }, newFlightPlan);
        }
        private async Task<IActionResult> AddObjects(dynamic bodyObj)
        {
            int passengers = bodyObj["passengers"];
            string companyName = bodyObj["company_name"];
            double longitude = bodyObj["initial_location"]["longitude"];
            double latitude = bodyObj["initial_location"]["latitude"];
            DateTime dateTime = bodyObj["initial_location"]["date_time"];
            dynamic segmentsObj = bodyObj["segments"];

            Flight newFlight = AddFlight();

            FlightPlan newFlightPlan = AddFlightPlan(newFlight, passengers, companyName);

            InitialLocation newInitialLocation = AddInitialLocation(newFlightPlan, longitude, latitude, dateTime);

            DateTime end = AddSegments(dateTime, segmentsObj, newFlightPlan);

            newFlightPlan.EndTime = end;

            return await UpdateDb(newFlight, newFlightPlan, newInitialLocation);
        }

        // POST: api/FlightPlans
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<IActionResult> PostFlightPlan(List<IFormFile> files)
        {

            var result = BuildString(files);
            dynamic bodyObj = ConvertAndDeserialize(result);

            return AddObjects(bodyObj);
        }

        private bool FlightPlanExists(int id)
        {
            return _flightContext.FlightPlanItems.Any(e => e.Id == id);
        }
        private bool FlightExists(int id)
        {
            return _flightContext.FlightItems.Any(e => e.Id == id);
        }
        private bool InitialLocationExists(int id)
        {
            return _flightContext.InitialLocationItems.Any(e => e.Id == id);
        }
        private bool SegmentExists(int id)
        {
            return _flightContext.SegmentItems.Any(e => e.Id == id);
        }
    }
}
