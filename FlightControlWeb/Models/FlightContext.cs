using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    /*
     * this class represent a DB of flights in our program 
     */
    public class FlightContext : DbContext, IDataContext
    {
        public FlightContext(DbContextOptions<FlightContext> options)
            : base(options)
        {
        }

        // this method returns all the flight details of our flights in the program
        public DbSet<Flight> FlightItems { get; set; }
        // this method returns all the flight plan details of our flights in the program
        public DbSet<FlightPlan> FlightPlanItems { get; set; }
        // this method returns all the starting location details of our flights 
        public DbSet<InitialLocation> InitialLocationItems { get; set; }
        // this method returns all the segments details of flights that our program has
        public DbSet<Segment> SegmentItems { get; set; }
        // this method returns all the server details that our program has
        public DbSet<Server> ServerItems { get; set; }
        // this method returns all the external flights that our program has
        public DbSet<Flight> ExternalFlightItems { get; set; }
        // this method saves changes that has been made in the DB 
        public Task SaveChangesAsync() => base.SaveChangesAsync();
    }
}
