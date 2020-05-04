using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public interface IServersManager
    {
        IEnumerable<Server> GetExternalServers();
        void AddExternalServer(Server s);
        void DeleteExternalServer(String id);
    }
}
