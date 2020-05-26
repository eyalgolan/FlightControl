using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public interface IFlightPlanManager
    {
        public FlightPlan AddFlightPlan(Flight newFlight, int passengers, string companyName);
    }
}
