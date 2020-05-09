using Microsoft.EntityFrameworkCore;

namespace FlightControlWeb.Models
{
    public class FlightContext : DbContext
    {
        public FlightContext(DbContextOptions<FlightContext> options)
            : base(options)
        {
        }

        public DbSet<Flight> TodoItems { get; set; }
    }
}
