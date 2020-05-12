using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FlightControlWeb.Models;

namespace FlightControlWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightsController : ControllerBase
    {
        private readonly FlightContext _context;

        public FlightsController(FlightContext context)
        {
            _context = context;
        }

        // GET: api/Flights
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Flight>>> GetTodoItems()
        {
            return await _context.FlightItems.ToListAsync();
        }

        // DELETE: api/Flights/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Flight>> DeleteFlight(string id)
        {
            //var flight = await _context.FlightItems.FindAsync(id);
            //await db.Foos.Where(x => x.UserId == userId).ToListAsync();
            var flight = await _context.FlightItems.Where(x => x.FlightId == id).FirstOrDefaultAsync();
            if (flight == null)
            {
                return NotFound();
            }

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
