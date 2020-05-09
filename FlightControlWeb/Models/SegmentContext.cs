using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace FlightControlWeb.Models
{
    public class SegmentContext : DbContext
    {
        public SegmentContext(DbContextOptions<SegmentContext> options)
            : base(options)
        {
        }

        public SegmentContext()
        {

        }

        public DbSet<Segment> SegmentItems { get; set; }
    }
}
