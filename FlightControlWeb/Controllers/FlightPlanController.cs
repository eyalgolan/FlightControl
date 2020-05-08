using System.Collections.Generic;
using FlightControlWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FlightControlWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightPlanController : ControllerBase
    {
        private readonly IFlightsManager _flightsManager = new FlightsManager();

        // GET: api/FlightPlan/5
        [HttpGet("{id}", Name = "Get")]
        public FlightPlan Get(string id)
        {
            var fp = _flightsManager.GetFlightById(id);
            return fp;
        }

        // POST: api/FlightPlan
        [HttpPost]
        public void Post([FromBody] string value)
        {
            // read file into a string and deserialize JSON to a type
            var flightPlan = JsonConvert.DeserializeObject<FlightPlan>(
                System.IO.File.ReadAllText("../wwwroot/json/FlightPlan.json"));

            FlightPlan flightPlan2;
            // deserialize JSON directly from a file
            using (var file =
                System.IO.File.OpenText("../wwwroot/json/FlightPlan.json"))
            {
                var serializer = new JsonSerializer();
                flightPlan2 = (FlightPlan) serializer.Deserialize(file, typeof(FlightPlan));
            }

            _flightsManager.AddFlightPlan(flightPlan2, false);
        }

        // PUT: api/FlightPlan/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}