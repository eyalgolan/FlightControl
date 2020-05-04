using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public class ServersManager : IServersManager
    {
        private static List<Server> servers = new List<Server>();
        public void AddExternalServer(Server s)
        {
            servers.Add(s);
        }

        public void DeleteExternalServer(string id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Server> GetExternalServers()
        {
            throw new NotImplementedException();
        }
    }
}
