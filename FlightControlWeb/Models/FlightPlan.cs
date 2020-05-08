using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FlightControlWeb.Models
{
    public class FlightPlan
    {
        public FlightPlan(int newPassenger, string newCompany, InitialLocation newLocation, IEnumerable<Segment> newSeg)
        {
            Passengers = newPassenger;
            CompanyName = newCompany;
            InitialLocation = newLocation;
            Segments = newSeg;
        }

        [Required] public int Passengers { get; set; }

        [Required] public string CompanyName { get; set; }

        [Required] public InitialLocation InitialLocation { get; set; }

        [Required] public IEnumerable<Segment> Segments { get; set; }
    }
}