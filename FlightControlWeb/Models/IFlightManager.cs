using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public interface IFlightManager
    {
        public IEnumerable<Flight> GetServerFlights(string dt);
        public IEnumerable<Flight> GetAllFlights(DateTime dt);
        public void AddFlightPlan(FlightPlan fp, bool isExternalInput);
        public FlightPlan GetFlightById(string id);
        public void DeleteFlight(Flight f);

        public void CreateId(Flight newFlight);
    }
}