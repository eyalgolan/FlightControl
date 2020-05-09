using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace FlightControlWeb.Models
{
    public class FlightPlanContext : DbContext
    {
        public FlightPlanContext(DbContextOptions<FlightPlanContext> options)
            : base(options)
        {
        }

        public FlightPlanContext()
        {

        }

        public DbSet<FlightPlan> FlightPlanItems { get; set; }
    }
}
