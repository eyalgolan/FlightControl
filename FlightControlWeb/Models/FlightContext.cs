using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

namespace FlightControlWeb.Models
{
    public class FlightContext : DbContext, IDataContext
    {
        public FlightContext(DbContextOptions<FlightContext> options)
            : base(options)
        {
        }


        public DbSet<Flight> FlightItems { get; set; }
        public DbSet<FlightPlan> FlightPlanItems { get; set; }
        public DbSet<InitialLocation> InitialLocationItems { get; set; }
        public DbSet<Segment> SegmentItems { get; set; }
        public DbSet<Server> ServerItems { get; set; }

        public Task SaveChangesAsync() => base.SaveChangesAsync();
    }
}
