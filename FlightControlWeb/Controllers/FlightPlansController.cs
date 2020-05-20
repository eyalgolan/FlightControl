using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlightControlWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace FlightControlWeb.Controllers
{
    [Route("api/FlightPlan")]
    [ApiController]
    public class FlightPlansController : ControllerBase
    {
        private readonly IDataContext _flightContext;
        private readonly IFlightManager _flightManager;
        private readonly IFlightPlanManager _flightPlanManager;

        public FlightPlansController(FlightContext flightContext)
        {
            _flightContext = flightContext;
            _flightPlanManager = new FlightPlanManager();
            _flightManager = new FlightManager();
        }

        private async Task<ActionResult<FlightPlanData>> BuildMatchingFlightPlan(string id, FlightPlan flightPlan)
        {
            var matchingFlight = await _flightContext.FlightItems.Where(x => x.FlightId == flightPlan.FlightId)
                .FirstOrDefaultAsync();
            var matchingInitialLocation = await _flightContext.InitialLocationItems
                .Where(x => x.FlightPlanId == flightPlan.Id).FirstOrDefaultAsync();
            IEnumerable<Segment> matchingSegments =
                await _flightContext.SegmentItems.Where(x => x.FlightPlanId == flightPlan.Id).ToListAsync();
            var flightPlanData = new FlightPlanData
            {
                passengers = flightPlan.Passengers,
                company_name = flightPlan.CompanyName,
                initial_location = matchingInitialLocation,
                segments = matchingSegments
            };

            return flightPlanData;
        }

        // GET: api/FlightPlans/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FlightPlanData>> GetFlightPlan(string id)
        {
            IEnumerable<FlightPlan> allFlightPlans = await _flightContext.FlightPlanItems.ToListAsync();

            var flightPlan = await _flightContext.FlightPlanItems.Where(x => x.FlightId == id).FirstOrDefaultAsync();

            if (flightPlan == null) return NotFound();

            return await BuildMatchingFlightPlan(id, flightPlan);
        }

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

        private dynamic ConvertAndDeserialize(StringBuilder result)
        {
            var input = result.ToString();
            dynamic bodyObj = JsonConvert.DeserializeObject(input);
            return bodyObj;
        }

        private Flight AddFlight()
        {
            var newFlight = new Flight();
            _flightManager.CreateId(newFlight);
            _flightContext.FlightItems.Add(newFlight);

            return newFlight;
        }

        private FlightPlan AddFlightPlan(Flight newFlight, int passengers, string companyName)
        {
            var newFlightPlan = new FlightPlan
            {
                FlightId = newFlight.FlightId,
                IsExternal = false
            };

            newFlightPlan.Passengers = passengers;
            newFlightPlan.CompanyName = companyName;
            _flightContext.FlightPlanItems.Add(newFlightPlan);

            return newFlightPlan;
        }

        private InitialLocation AddInitialLocation(FlightPlan newFlightPlan, double longitude, double latitude,
            DateTime dateTime)
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

        private DateTime AddSegments(DateTime dateTime, dynamic segmentsObj, FlightPlan newFlightPlan)
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

            return CreatedAtAction("GetFlightPlan", new {id = newFlightPlan.FlightId}, newFlightPlan);
        }

        private async Task<IActionResult> AddObjects(dynamic bodyObj)
        {
            int passengers = bodyObj["passengers"];
            string companyName = bodyObj["company_name"];
            double longitude = bodyObj["initial_location"]["longitude"];
            double latitude = bodyObj["initial_location"]["latitude"];
            DateTime dateTime = bodyObj["initial_location"]["date_time"];
            var segmentsObj = bodyObj["segments"];

            var newFlight = AddFlight();

            var newFlightPlan = AddFlightPlan(newFlight, passengers, companyName);

            var newInitialLocation = AddInitialLocation(newFlightPlan, longitude, latitude, dateTime);

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
            var bodyObj = ConvertAndDeserialize(result);

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
    }
}