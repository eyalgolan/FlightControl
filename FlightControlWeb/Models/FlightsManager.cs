using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace FlightControlWeb.Models
{
    public class FlightsManager : IFlightsManager
    {
        private static readonly FlightPlan fp = new FlightPlan(30, "s", new InitialLocation(50, 50, new DateTime()),
            new List<Segment>());

        private static readonly FlightPlan fp1 = new FlightPlan(40, "a", new InitialLocation(10, 10, new DateTime()),
            new List<Segment>());

        private static readonly List<Flight> flights = new List<Flight>
        {
            new Flight(fp, false),
            new Flight(fp1, false)
        };

        private ConcurrentDictionary<int, FlightPlan> flightPlanDictionary = new ConcurrentDictionary<int, FlightPlan>();
        public void AddFlightPlan(FlightPlan fp, bool isExternalInput)
        {
            var newFlight = new Flight(fp, isExternalInput);
            flights.Add(newFlight);
        }

        public void DeleteFlight(Flight f)
        {
            var toRemove = flights.Where(x => x.FlightId == f.FlightId).FirstOrDefault();
            if (toRemove == null) throw new Exception("flight not found");
            flights.Remove(toRemove);
        }

        public IEnumerable<Flight> GetAllFlights(string dt)
        {
            return flights;
        }

        public FlightPlan GetFlightById(string id)
        {
            var toReturn = flights.Where(x => x.FlightId == id).FirstOrDefault().Fp;

            if (toReturn == null) throw new Exception("Flight not found.");
            return toReturn;
        }

        public IEnumerable<Flight> GetServerFlights(string dt)
        {
            throw new NotImplementedException();
        }
    }
}