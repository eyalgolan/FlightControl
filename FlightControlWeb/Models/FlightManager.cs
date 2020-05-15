using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public class FlightManager : IFlightManager
    {
        public void CreateId(Flight newFlight)
        {
            const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var rand = new Random();

            var id = "";
            for (var i = 0; i < 2; i++)
            {
                var j = rand.Next(0, letters.Length - 1);
                id += letters[j];
            }

            id += rand.Next(1000, 99999999).ToString();

            newFlight.FlightId = id;
        }

        public void AddFlightPlan(FlightPlan fp, bool isExternalInput)
        {
            throw new NotImplementedException();
        }

        public void DeleteFlight(Flight f)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Flight> GetAllFlights(DateTime dt)
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

        public double GetFlightLatitude(Flight f)
        {
            throw new NotImplementedException();
        }

        public double GetFlightLongitude(Flight f)
        {
            throw new NotImplementedException();
        }
    }
}
