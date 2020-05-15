using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace FlightControlWeb.Models
{
    interface IDataContext
    {
        //DbSet<TEntity> Set<TEntity>() where TEntity : class;
        //Task<int> SaveChangesAsync();

        public DbSet<Flight> FlightItems { get; set; }
        public DbSet<FlightPlan> FlightPlanItems { get; set; }
        public DbSet<InitialLocation> InitialLocationItems { get; set; }
        public DbSet<Segment> SegmentItems { get; set; }

        Task SaveChangesAsync();
    }
}
