using System;

namespace FlightControlWeb.Models
{
    public class Flight
    {
        private FlightPlan flightPlan;
        private bool isExternal;

        public Flight(FlightPlan fpInput, bool isExternalInput)
        {
            flightPlan = fpInput;
            isExternal = isExternalInput;
            FlightId = CreateID();
        }

        public string FlightId { get; set; }

        public double Longtitude { get; set; }
        public double Latitude { get; set; }
        public int Passengers { get; set; }
        public string CompanyName { get; set; }
        public string DateTime { get; set; }
        public bool IsExternal { get; set; }
        public FlightPlan Fp { get; set; }

        private string CreateID()
        {
            var letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
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