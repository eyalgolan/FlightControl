using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public class Flight
    {
        public String FlightId { get; set; }
        public double Longtitude { get; set; }
        public double Latitude { get; set; }
        public int Passengers { get; set; }
        public String CompanyName { get; set; }
        public String DateTime { get; set; }
        public Boolean IsExternal { get; set; }
    }
}
