using System;

namespace FlightControlWeb.Models
{
    public class InitialLocation
    {
        public InitialLocation(double newLong, double newLat, DateTime newDate)
        {
            Longtitude = newLong;
            Latitude = newLat;
            DateTime = newDate;
        }

        public double Longtitude { get; set; }

        public double Latitude { get; set; }

        public DateTime DateTime { get; set; }
    }
}