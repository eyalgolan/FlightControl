using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public class FlightManager : IFlightManager
    {
        // put here the hardcoded flights.
        public void CreateId(Flight newFlight)
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

            newFlight.FlightId = id;
        }

        public void AddFlightPlan(FlightPlan fp, bool isExternalInput)
        {
            throw new NotImplementedException();
        }

        public void DeleteFlight(Flight f)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Flight> GetAllFlights(DateTime dt)
        {
            throw new NotImplementedException();
        }

        public FlightPlan GetFlightById(string id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Flight> GetServerFlights(string dt)
        {
            throw new NotImplementedException();
        }

        public double GetFlightLatitude(Flight f)
        {
            throw new NotImplementedException();
        }

        public double GetFlightLongitude(Flight f)
        {
            throw new NotImplementedException();
        }
    }
}

//private static Segment s1 = new Segment(18.220554, -63.068615, 1000);
//private static Segment s2 = new Segment(36.204824, 138.252924, 2000);
//private static Segment s3 = new Segment(38.7441, -9.1581, 1500);
//private static Segment s4 = new Segment(59.9137, 10.7390, 7899);
//private static Segment s5 = new Segment(-35.62, 27.31, 1200);
//private static Segment s6 = new Segment(-32.981, -55.789, 1200);
//private static Segment s7 = new Segment(-1.571, -83.694, 1200);
//private static Segment s8 = new Segment(55.7252, 37.6290, 4837);
//private static Segment s9 = new Segment(28.5635, 77.1528, 1165);
//private static Segment s10 = new Segment(14.5985, 120.9839, 2861);
//private static Segment s11 = new Segment(-35.41, 120.24, 5846);
//private static List<Segment> segList1 = new List<Segment>() { s1, s2 };
//private static List<Segment> segList3 = new List<Segment>() { s3, s4 };
//private static List<Segment> segList5 = new List<Segment>() { s5, s6, s7 };
//private static List<Segment> segList8 = new List<Segment>() { s8, s9, s10, s11 };


//private static FlightPlan fp0 = new FlightPlan(300, "EL-AL", new InitialLocation(56.130366, -106.346771, DateTime.Now), segList1);
//private static FlightPlan fp1 = new FlightPlan(240, "British Airways", new InitialLocation(40.6971, -73.9796, DateTime.Now), segList3);
//private static FlightPlan fp2 = new FlightPlan(130, "Maldives Airways", new InitialLocation(31.438, 35.074, DateTime.Now), segList5);
//private static FlightPlan fp3 = new FlightPlan(403, "United Airlines", new InitialLocation(51.5076, -0.1276, DateTime.Now), segList8);
//private static FlightPlan fp4 = new FlightPlan(320, "Ryan Air", new InitialLocation(-36.204824, 138.252924, DateTime.Now), segList1);
//private static FlightPlan fp5 = new FlightPlan(140, "Wizz Air", new InitialLocation(28, 120, DateTime.Now), segList3);
//private static FlightPlan fp6 = new FlightPlan(130, "Easy Jet", new InitialLocation(50, 140, DateTime.Now), segList5);
//private static FlightPlan fp7 = new FlightPlan(406, "EL-AL", new InitialLocation(42, 39, DateTime.Now), segList8);
//private static FlightPlan fp8 = new FlightPlan(310, "British Airways", new InitialLocation(50, 50, DateTime.Now), segList1);
//private static List<Flight> allFlights = new List<Flight>()
//        {
//            new Flight(fp0, false),
//            new Flight(fp1, true),
//            new Flight(fp2, false),
//            new Flight(fp3, true),
//            new Flight(fp4, false),
//            new Flight(fp5, true),
//            new Flight(fp6, false),
//            new Flight(fp7, true),
//            new Flight(fp8, false),
//        };

