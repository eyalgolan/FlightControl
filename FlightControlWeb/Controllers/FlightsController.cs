using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using FlightControlWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        public async Task<IEnumerable<Flight>> GetFlights([FromQuery] DateTime relative_to, bool? sync_all = false)
        {
            relative_to = relative_to.ToUniversalTime();
            bool syncAll = sync_all.HasValue ? sync_all.Value : false;

            IEnumerable<InitialLocation> relaventInitials =
                await _flightContext.InitialLocationItems.Where(x => x.DateTime < relative_to).ToListAsync();
            IEnumerable<FlightPlan> relaventPlans = Enumerable.Empty<FlightPlan>();
            foreach (var initial in relaventInitials)
            {
                int segmentFlightPlanId = initial.FlightPlanId;
                var relaventPlan = await _flightContext.FlightPlanItems.FindAsync(segmentFlightPlanId);
                if (relaventPlan != null) relaventPlans = relaventPlans.Append(relaventPlan);
            }

            IEnumerable<Flight> relaventFlights = new List<Flight>();
            foreach (var plan in relaventPlans)
                if (plan.EndTime > relative_to)
                {
                    var relaventFlight = await _flightContext.FlightItems.Where(x=>x.FlightId == plan.FlightId).FirstOrDefaultAsync();
                    if (relaventFlight != null)
                    {
                        var currentPlan = await _flightContext.FlightPlanItems.Where(x => x.FlightId == relaventFlight.FlightId).FirstOrDefaultAsync();
                        IEnumerable<Segment> planSegments = await _flightContext.SegmentItems
                            .Where(x => x.FlightPlanId == currentPlan.Id).ToListAsync();
                        //relaventFlight.CurrentLatitude = _flightManager.GetFlightLatitude(relaventFlight);
                        //relaventFlight.CurrentLongitude = _flightManager.GetFlightLongitude(relaventFlight);
                        relaventFlight.CompanyName = plan.CompanyName;
                        relaventFlight.CurrentDateTime = relative_to;
                        relaventFlights = relaventFlights.Append(relaventFlight);
                    }
                }

            //todo need to check this works
            if (syncAll)
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
                            IEnumerable<Flight> response = result.Content.ReadAsAsync<IEnumerable<Flight>>().Result;
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
        /*
        private void LinearInterpolation(FlightPlan fp, DateTime utcDate)
        {
            var segments = fp.Segments.ToList();

            int totalTimeSpan = segments.Sum(v => v.Timespan_seconds);
            TimeSpan dif = utcDate.Subtract(fp.Initial_Location.Date_Time);
            int dif_sec = (int)dif.TotalSeconds;
            if (totalTimeSpan > dif_sec)
            {
                double cameFromLati = fp.Initial_Location.Latitude;
                double cameFromLong = fp.Initial_Location.Longitude;
                foreach (Segment element in flight.Segments)
                {

                    if (dif_sec >= element.Timespan_seconds)
                    {
                        dif_sec = dif_sec - element.Timespan_seconds;
                    }
                    else
                    {
                        double relative = (double)dif_sec / (double)element.Timespan_seconds;
                        var flights = (IDictionary<string, Flight>)_cache.Get("flights");
                        flights[fp.ID].Latitude = cameFromLati + (relative * (element.Latitude - cameFromLati));
                        flights[fp.ID].Longitude = cameFromLong + (relative * (element.Longitude - cameFromLong));
                        break;
                    }
                    cameFromLati = element.Latitude;
                    cameFromLong = element.Longitude;
                }
            }
        }*/
    }
}