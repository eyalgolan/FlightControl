using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    interface IFlightsManager
    {
        Flight[] GetServerFlights(DateTime dt);
        Flight[] GetAllFlights(DateTime dt);
        void AddFlightPlan(FlightPlan fp);
        FlightPlan GetFlight(int id);
        void DeleteFlight(Flight f);
    }
}
