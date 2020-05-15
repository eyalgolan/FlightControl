using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public class MockContext : IDataContext
    {
        public DbSet<Flight> FlightItems { get; set; }
        public DbSet<FlightPlan> FlightPlanItems { get; set; }
        public DbSet<InitialLocation> InitialLocationItems { get; set; }
        public DbSet<Segment> SegmentItems { get; set; }

        public Task SaveChangesAsync()
        {
            throw new NotImplementedException();
        }
    }
}
