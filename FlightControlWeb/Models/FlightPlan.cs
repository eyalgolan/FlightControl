using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public class FlightPlan
    {
        public int Passengers { get; set; }
        public String CompanyName { get; set; }
        public int initialLogtitude { get; set; }
        public int initialLatitude { get; set; }
        public int initialDateTime { get; set; }

        public List<Segment> segments;

        // todo add relavent fields
    }
}
