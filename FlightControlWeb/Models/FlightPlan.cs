using System.Collections.Generic;

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

        public int Passengers { get; set; }

        public string CompanyName { get; set; }

        public InitialLocation InitialLocation { get; set; }

        public IEnumerable<Segment> Segments { get; set; }
    }
}