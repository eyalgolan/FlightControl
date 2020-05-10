using System.Collections;
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
    public class FlightPlansController : ControllerBase
    {
        private readonly FlightPlanContext _flightPlanContext;
        private readonly IFlightPlanManager _flightPlanManager;

        public FlightPlansController(FlightPlanContext flightPlanContext)
        {
            _flightPlanContext = flightPlanContext;
            _flightPlanManager = new FlightPlanManager();
        }

        // GET: api/FlightPlans/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FlightPlan>> GetFlightPlan(string id)
        {
            IEnumerable<FlightPlan> allFlightPlans = await _flightPlanContext.FlightPlanItems.ToListAsync();

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
        public async Task<ActionResult<FlightPlan>> PostFlightPlan([FromBody] FlightPlan flightPlan)
        {

            _flightPlanContext.FlightPlanItems.Add(flightPlan);
            try
            {
                await _flightPlanContext.SaveChangesAsync();
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

        private bool FlightPlanExists(string id)
        {
            return _flightPlanContext.FlightPlanItems.Any(e => e.FlightId == id);
        }
    }
}
