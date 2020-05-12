using System;
using System.Collections.Generic;
using System.Linq;

namespace FlightControlWeb.Models
{
    public class FlightPlanManager : IFlightPlanManager
    {
        private static int flightPlanCount = 0;
        private static int initialLocationCount = 0;
        public FlightPlan AddFlightPlan(FlightPlan inputFlightPlan)
        {
            Flight newFlight = new Flight();
            throw new NotImplementedException();
        }

        public FlightPlan GetFlightPlanByFlightId(IEnumerable<FlightPlan> allFlightPlans, string inputFlightId)
        {
            var flightPlan = allFlightPlans.Single(x => x.FlightId == inputFlightId);

            return flightPlan;
        }

        public void CreateId(FlightPlan newFlightPlan)
        {
            newFlightPlan.Id = flightPlanCount;
            flightPlanCount++;
        }

        public void CreateId(InitialLocation newInitialLocation)
        {
            newInitialLocation.Id = initialLocationCount;
            initialLocationCount++;
        }
    }
}