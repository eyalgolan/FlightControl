using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public interface IFlightManager
    {
        IEnumerable<Flight> GetServerFlights(string dt);
        IEnumerable<Flight> GetAllFlights(string dt);
        void AddFlightPlan(FlightPlan fp, bool isExternalInput);
        FlightPlan GetFlightById(string id);
        void DeleteFlight(Flight f);
    }
}