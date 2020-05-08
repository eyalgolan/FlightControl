using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public class ServersManager : IServersManager
    {
        private static List<Server> allServers = new List<Server>();

        public IEnumerable<Server> GetAllServers()
        {
            return allServers;
        }

        public Server GetServerByID(string id)
        {
            Server tempS = allServers.Where(x => x.ServerID == id).FirstOrDefault();
            if (tempS == null)
            {
                throw new Exception("Server not found.");
            }

            return tempS;
        }

        public void AddServer(Server server)
        {
            allServers.Add(server);
        }

        public void DeleteServer(string id)
        {
            Server tempS = allServers.Where(x => x.ServerID == id).FirstOrDefault();
            if (tempS == null)
            {
                throw new Exception("Server not found.");
            }

            allServers.Remove(tempS);
        }
    }
}
