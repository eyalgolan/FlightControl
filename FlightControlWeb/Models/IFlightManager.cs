using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public interface IFlightManager
    {

        public void CreateId(Flight newFlight);

        public Flight AddFlight();
    }
}