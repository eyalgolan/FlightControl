using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    interface IFlightsManager
    {
        IEnumerable<Flight> GetServerFlights(String dt);
        IEnumerable<Flight> GetAllFlights(String dt);
        void AddFlightPlan(FlightPlan fp);
        FlightPlan GetFlightById(String id);
        void DeleteFlight(Flight f);
    }
}
