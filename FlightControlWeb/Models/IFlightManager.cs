using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    /*
     * this interface represent every flight we will have in our program.
     * it contains method that should generates flight id and a method
     * that adds new flight.
     */
    public interface IFlightManager
    {

        public void CreateId(Flight newFlight);

        public Flight AddFlight();
    }
}