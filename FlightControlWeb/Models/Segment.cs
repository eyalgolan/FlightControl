using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlightControlWeb.Models
{
    public class Segment
    {
        [Key] public int Id { get; set; }
        [ForeignKey("FlightPlan")] public int FlightPlanId { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public string TimeSpanSeconds { get; set; }
    }
}
