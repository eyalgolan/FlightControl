namespace FlightControlWeb.Models
{
    /*
     * this interface represent every flight we will have in our program.
     * it contains method that should generates flight id and a method
     * that adds new flight.
     */
    public interface IFlightManager
    {
        // this member should generate an id for the given flight
        public void CreateId(Flight newFlight);

        // this member should create a new flight and add it to our DB
        public Flight AddFlight();
    }
}