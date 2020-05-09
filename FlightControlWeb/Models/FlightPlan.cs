using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FlightControlWeb.Models
{
    public class FlightPlan
    {
        [Key] public string FlightId { get; set; }
        public int Passengers { get; set; }
        public string CompanyName { get; set; }

        public InitialLocation InitLocation { get; set; }

        public IEnumerable<Segment> Segments { get; set; }
    }
}
