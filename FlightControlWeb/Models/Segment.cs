using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlightControlWeb.Models
{
    /*
     * this class represent a segment as we save it in our DBs
     */
    public class Segment
    {
        [Key] public int Id { get; set; }
        [ForeignKey("FlightPlan")] public int FlightPlanId { get; set; }
        [Required] public double Longitude { get; set; }
        [Required] public double Latitude { get; set; }
        [Required] public int TimeSpanSeconds { get; set; }
        [Required] public DateTime StartTime { get; set; }
        [Required] public DateTime EndTime { get; set; }
    }
}
