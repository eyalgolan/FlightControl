using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlightControlWeb.Models
{
    public class InitialLocation
    {
        [Key] public int Id { get; set; }
        [ForeignKey("FlightPlan")] public int FlightPlanId { get; set; }
        [Required] public double Longitude { get; set; }
        [Required] public double Latitude { get; set; }
        [Required] public DateTime DateTime { get; set; }

    }
}
