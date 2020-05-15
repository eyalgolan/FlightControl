using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly FlightContext _context;

        public FlightsController(FlightContext context)
        {
            _context = context;
        }

        /*
        // GET: api/Flights/
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Segment>>> GetTodoItems()
        {
            return await _context.SegmentItems.ToListAsync();
        }
        */

        
        // GET: api/Flights
        [HttpGet]
        public async Task<IEnumerable<Flight>> GetFlights([FromQuery] DateTime relative_to)
        {
            IEnumerable<InitialLocation> relaventInitials =
                await _context.InitialLocationItems.Where(x => x.DateTime < relative_to).ToListAsync();
            IEnumerable<FlightPlan> relaventPlans = Enumerable.Empty<FlightPlan>();
            foreach (var initial in relaventInitials)
            {
                int segmentFlightPlanId = initial.FlightPlanId;
                var relaventPlan = await _context.FlightPlanItems.FindAsync(segmentFlightPlanId);
                if (relaventPlan != null) relaventPlans = relaventPlans.Append(relaventPlan);
            }

            IEnumerable<Flight> relaventFlights = new List<Flight>();
            foreach (var plan in relaventPlans)
                if (plan.EndTime > relative_to)
                {
                    var relaventFlight = await _context.FlightItems.FindAsync(plan.FlightId);
                    if (relaventFlight != null) relaventFlights = relaventFlights.Append(relaventFlight);
                }

            return relaventFlights;
        }
        
        // DELETE: api/Flights/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Flight>> DeleteFlight(string id)
        {
            //var flight = await _context.FlightItems.FindAsync(id);
            //await db.Foos.Where(x => x.UserId == userId).ToListAsync();
            var flight = await _context.FlightItems.Where(x => x.FlightId == id).FirstOrDefaultAsync();
            if (flight == null) return NotFound();

            var flightPlan = await _context.FlightPlanItems.Where(x => x.FlightId == id).FirstOrDefaultAsync();
            _context.FlightPlanItems.Remove(flightPlan);
            _context.FlightItems.Remove(flight);
            await _context.SaveChangesAsync();

            return flight;
        }

        private bool FlightExists(string id)
        {
            return _context.FlightItems.Any(e => e.FlightId == id);
        }
    }
}