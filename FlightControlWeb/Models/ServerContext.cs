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
