using System;
using System.ComponentModel.DataAnnotations;

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
            FlightId = CreateId();
        }

        [Required] public string FlightId { get; set; }

        [Required] public double Longtitude { get; set; }

        [Required] public double Latitude { get; set; }

        [Required] public int Passengers { get; set; }

        [Required] public string CompanyName { get; set; }

        [Required] public string DateTime { get; set; }

        [Required] public bool IsExternal { get; set; }

        [Required] public FlightPlan Fp { get; set; }

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