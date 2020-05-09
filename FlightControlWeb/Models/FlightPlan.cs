using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public class FlightPlan
    {
        [Key] public string FlightId { get; set; }
        public int Passengers { get; set; }
        public string CompanyName { get; set; }

        public string InitialLongtitude { get; set; }

        public string InitialLatitude { get; set; }

        public IEnumerable<Segment> Segments { get; set; }
    }
}
