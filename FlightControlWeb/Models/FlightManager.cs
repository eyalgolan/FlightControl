using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    /*
     * this class is a local implementation of IFlightManager
     */
    public class FlightManager : IFlightManager
    {
        // this method generates an external id for each flight
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
        /*
         * This method creates a new Flight and add it to our DBs.
         * then, it returns the new made flight object.
         */
        public Flight AddFlight()
        {
            var newFlight = new Flight();
            CreateId(newFlight);

            return newFlight;
        }
    }
}

