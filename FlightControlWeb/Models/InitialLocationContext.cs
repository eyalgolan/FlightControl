using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace FlightControlWeb.Models
{
    public class InitialLocationContext : DbContext
    {
        public InitialLocationContext(DbContextOptions<InitialLocationContext> options)
            : base(options)
        {
        }

        public InitialLocationContext()
        {

        }

        public DbSet<InitialLocation> FlightPlanItems { get; set; }
    }
}
