using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public class FlightPlanRequest
    {
        public int Passengers { get; set; }
        public string CompanyName { get; set; }
        public InitialLocation InitialLocation { get; set; }
        public IEnumerable<Segment> Segments { get; set; }
    }
}
