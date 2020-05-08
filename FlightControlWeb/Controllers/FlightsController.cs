using System.Collections.Generic;
using FlightControlWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace FlightControlWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightsController : ControllerBase
    {
        private readonly IFlightsManager _flightManager = new FlightsManager();

        public FlightsController(IFlightsManager fm)
        {
            _flightManager = fm;
        }

        // GET: api/Flights?relative_to=<DATE_TIME> api/Flights?relative_to=<DATE_TIME>&syc_all
        [HttpGet]
        public IEnumerable<Flight> GetAllFlights()
        {
            var dt = "";
            return _flightManager.GetAllFlights(dt);
        }

        // GET: api/Flights/5
        [HttpGet("{id}", Name = "Get")]
        public FlightPlan Get(string id)
        {
            var fp = _flightManager.GetFlightById(id);
            return fp;
        }

        // POST: api/FlightPlan
        [HttpPost]
        public FlightPlan PostFlightPlan(FlightPlan fp, bool isExternal)
        {
            // 
            _flightManager.AddFlightPlan(fp, isExternal);
            return fp;
        }

        // DELETE: api/ApiWithActions/5
        // we want DELETE: /api/Flights/{id}
        [HttpDelete("{id}")]
        public void DeleteFlightById(int id)
        {
        }
    }
}