using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    interface IServerManager
    {
        Server[] GetExternalServers();
        void AddExternalServer(Server s);
        void DeleteExternalServer(String id);
    }
}
