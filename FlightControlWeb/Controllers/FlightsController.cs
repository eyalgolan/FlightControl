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
            return await _context.TodoItems.ToListAsync();
        }

        // GET: api/Flights/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Flight>> GetFlight(string id)
        {
            var flight = await _context.TodoItems.FindAsync(id);

            if (flight == null)
            {
                return NotFound();
            }

            return flight;
        }

        // PUT: api/Flights/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFlight(string id, Flight flight)
        {
            if (id != flight.FlightId)
            {
                return BadRequest();
            }

            _context.Entry(flight).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FlightExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Flights
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Flight>> PostFlight(Flight flight)
        {
            _context.TodoItems.Add(flight);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (FlightExists(flight.FlightId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetFlight", new { id = flight.FlightId }, flight);
        }

        // DELETE: api/Flights/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Flight>> DeleteFlight(string id)
        {
            var flight = await _context.TodoItems.FindAsync(id);
            if (flight == null)
            {
                return NotFound();
            }

            _context.TodoItems.Remove(flight);
            await _context.SaveChangesAsync();

            return flight;
        }

        private bool FlightExists(string id)
        {
            return _context.TodoItems.Any(e => e.FlightId == id);
        }
    }
}
