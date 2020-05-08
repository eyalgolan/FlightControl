using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlightControlWeb.Model;

namespace FlightControlWeb.Models
{
    public interface IServersManager
    {
        IEnumerable<Server> GetAllServers();
        void AddServer(Server s);
        void DeleteServer(String id);
    }
}
