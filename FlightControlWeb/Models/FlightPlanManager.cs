namespace FlightControlWeb.Models
{
    /*
     * This method defines a new Flight Plan Manager
     * and it creates new internal flights
     */
    public class FlightPlanManager : IFlightPlanManager
    {
        /*
         * This method defines a new Flight Plan and add it to our DBs.
         * then, it returns the new made flight plan object.
         */
        public FlightPlan AddFlightPlan(Flight newFlight, int passengers, string companyName)
        {
            var newFlightPlan = new FlightPlan
            {
                FlightId = newFlight.FlightId,
                IsExternal = false,
                OriginServer = "-1"
            };

            newFlightPlan.Passengers = passengers;
            newFlightPlan.CompanyName = companyName;

            return newFlightPlan;
        }
    }
}