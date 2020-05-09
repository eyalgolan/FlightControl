using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FlightControlWeb.Models;

namespace FlightControlWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightPlansController : ControllerBase
    {
        private readonly FlightPlanContext _context;

        public FlightPlansController(FlightPlanContext context)
        {
            _context = context;
        }

        // GET: api/FlightPlans
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FlightPlan>>> GetFlightPlanItems()
        {
            return await _context.FlightPlanItems.ToListAsync();
        }

        // GET: api/FlightPlans/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FlightPlan>> GetFlightPlan(string id)
        {
            var flightPlan = await _context.FlightPlanItems.FindAsync(id);

            if (flightPlan == null)
            {
                return NotFound();
            }

            return flightPlan;
        }

        // PUT: api/FlightPlans/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFlightPlan(string id, FlightPlan flightPlan)
        {
            if (id != flightPlan.FlightId)
            {
                return BadRequest();
            }

            _context.Entry(flightPlan).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FlightPlanExists(id))
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

        // POST: api/FlightPlans
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<FlightPlan>> PostFlightPlan(FlightPlan flightPlan)
        {
            _context.FlightPlanItems.Add(flightPlan);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (FlightPlanExists(flightPlan.FlightId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetFlightPlan", new { id = flightPlan.FlightId }, flightPlan);
        }

        // DELETE: api/FlightPlans/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<FlightPlan>> DeleteFlightPlan(string id)
        {
            var flightPlan = await _context.FlightPlanItems.FindAsync(id);
            if (flightPlan == null)
            {
                return NotFound();
            }

            _context.FlightPlanItems.Remove(flightPlan);
            await _context.SaveChangesAsync();

            return flightPlan;
        }

        private bool FlightPlanExists(string id)
        {
            return _context.FlightPlanItems.Any(e => e.FlightId == id);
        }
    }
}
