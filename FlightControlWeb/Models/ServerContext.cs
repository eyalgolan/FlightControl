using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace FlightControlWeb.Models
{
    public class ServerContext : DbContext
    {
        public ServerContext(DbContextOptions<ServerContext> options)
            : base(options)
        {
        }

        public ServerContext()
        {

        }

        public DbSet<Server> TodoItems { get; set; }
    }
}
