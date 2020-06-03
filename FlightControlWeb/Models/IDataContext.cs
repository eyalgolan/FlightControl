using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    /*
     * this interface dictates the way our DBs are organized from inside
     */
    public interface IDataContext
    {
        // this member should contain the flights data in the DB
        public DbSet<Flight> FlightItems { get; set; }
        // this member should contain the flight plans data in the DB
        public DbSet<FlightPlan> FlightPlanItems { get; set; }
        // this member should contain the flights starting locations data in the DB
        public DbSet<InitialLocation> InitialLocationItems { get; set; }
        // this member should contain the flight's segments data in the DB
        public DbSet<Segment> SegmentItems { get; set; }
        // this member should contain the external flights data in the DB
        public DbSet<Flight> ExternalFlightItems { get; set; }
        // this member should contain the servers data in the DB
        public DbSet<Server> ServerItems { get; set; }
        // this task saves changes in our DBs (async)
        Task SaveChangesAsync();
    }
}
