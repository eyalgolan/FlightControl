using System;
using System.ComponentModel.DataAnnotations;

namespace FlightControlWeb.Models
{
    public class InitialLocation
    {
        [Key] public int FlightId { get; set; }
        public double Longtitude { get; set; }

        public double Latitude { get; set; }

        public DateTime DateTime { get; set; }
    }
}
