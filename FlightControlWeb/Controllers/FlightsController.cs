using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FlightControlWeb.Models;

namespace FlightControlWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightsController : ControllerBase
    {
        private IFlightsManager flightManager;

        public FlightsController(IFlightsManager fm)
        {
            this.flightManager = fm;
        }
        // GET: api/Flights?relative_to=<DATE_TIME> api/Flights?relative_to=<DATE_TIME>&syc_all
        [HttpGet]
        public IEnumerable<string> GetAllFlights()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Flights/5
        [HttpGet("{id}", Name = "Get")]
        public FlightPlan Get(int id)
        {
            FlightPlan fp = this.flightManager.GetFlightById(id);
            return fp;
        }

        // POST: api/FlightPlan
        [HttpPost]
        public FlightPlan PostFlightPlan(FlightPlan fp)
        {
            // 
            this.flightManager.AddFlightPlan(fp);
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
