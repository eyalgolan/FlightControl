using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    interface IFlightPlanManager
    {
        public FlightPlan AddFlightPlan(FlightPlan inputFlightPlan);
        public FlightPlan GetFlightPlanByFlightId(IEnumerable<FlightPlan> allFlightPlans, string inputFlightId);

        public void CreateId(FlightPlan newFlightPlan);

        public void CreateId(InitialLocation newInitialiLocation);
    }
}
