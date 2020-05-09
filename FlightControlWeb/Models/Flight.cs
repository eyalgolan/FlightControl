using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public class Flight
    {
        public string FlightId { get; set; }

        public double Longtitude { get; set; }

        public double Latitude { get; set; }

        public bool IsExternal { get; set; }

        //public FlightPlan Fp { get; set; }

        private static string CreateId()
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
            return id;
        }
    }
}
