using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public class FlightsManager : IFlightsManager
    {
        public void AddFlightPlan(FlightPlan fp)
        {
            throw new NotImplementedException();
        }

        public void DeleteFlight(Flight f)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Flight> GetAllFlights(string dt)
        {
            throw new NotImplementedException();
        }

        public FlightPlan GetFlightById(string id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Flight> GetServerFlights(string dt)
        {
            throw new NotImplementedException();
        }
    }
}
