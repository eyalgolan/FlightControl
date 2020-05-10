using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlightControlWeb.Models
{
    public class Segment
    {
        [Key] public int Id { get; set; }
        [ForeignKey("FlightPlan")] public int FlightPlanId { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public int TimeSpanSeconds { get; set; }
    }
}
