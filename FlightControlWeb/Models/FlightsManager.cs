using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public class FlightsManager : IFlightsManager
    {
        private static List<Flight> flights = new List<Flight>();

        public void AddFlightPlan(FlightPlan fp)
        {
            Flight newFlight = new Flight(fp);
            flights.Add(newFlight);
        }

        public void DeleteFlight(Flight f)
        {
            Flight toRemove = flights.Where(x => x.FlightId == f.FlightId).FirstOrDefault();
            if (toRemove == null)
            {
                throw new Exception("flight not found");
            }
            flights.Remove(toRemove);
        }

        public IEnumerable<Flight> GetAllFlights(string dt)
        {
            throw new NotImplementedException();
        }

        public FlightPlan GetFlightById(string id)
        {
            //FlightPlan toRemove = flightPlans.Where(x => x.FlightId == f.FlightId).FirstOrDefault();
            //if (toRemove == null)
            //{
            //    throw new Exception("flightPlan not found");
            //}
            //flightPlans.Remove(toRemove);
            throw new NotImplementedException();
        }

        public IEnumerable<Flight> GetServerFlights(string dt)
        {
            throw new NotImplementedException();
        }
    }
}
