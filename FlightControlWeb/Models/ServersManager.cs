using System;
using System.Collections.Generic;
using System.Linq;
using FlightControlWeb.Model;

namespace FlightControlWeb.Models
{
    public class ServersManager : IServersManager
    {
        private static readonly List<Server> allServers = new List<Server>();

        public IEnumerable<Server> GetAllServers()
        {
            return allServers;
        }

        public void AddServer(Server server)
        {
            allServers.Add(server);
        }

        public void DeleteServer(string id)
        {
            var tempS = allServers.Where(x => x.ServerID == id).FirstOrDefault();
            if (tempS == null) throw new Exception("Server not found.");

            allServers.Remove(tempS);
        }

        public Server GetServerByID(string id)
        {
            var tempS = allServers.Where(x => x.ServerID == id).FirstOrDefault();
            if (tempS == null) throw new Exception("Server not found.");

            return tempS;
        }
    }
}