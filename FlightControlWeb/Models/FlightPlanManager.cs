using System;
using System.Collections.Generic;
using System.Linq;

namespace FlightControlWeb.Models
{
    public class FlightPlanManager : IFlightPlanManager
    {
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
    }
}