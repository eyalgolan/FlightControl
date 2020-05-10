using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlightControlWeb.Models
{
    public class InitialLocation
    {
        [Key] public int Id { get; set; }
        [ForeignKey("FlightPlan")] public int FlightPlanId { get; set; }
        public double Longitude { get; set; }

        public double Latitude { get; set; }

        public DateTime DateTime { get; set; }
    }
}
