using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlightControlWeb.Models
{
    /*
     * this class contains all the data of our initial location.
     * it has the basic data of longitude latitude and date time and also
     * has a connection to flight plan using flight plan id
     */
    public class InitialLocation
    {
        // this member is the inner key for any initial location in the DB
        [Key] public int Id { get; set; }
        // this member is the external key for any initial location in the DB
        // it connects between flight plan to initial location
        [ForeignKey("FlightPlan")] public int FlightPlanId { get; set; }
        // this member represent the initial location's longitude
        [Required] public double Longitude { get; set; }
        // this member represent the initial location's latitude
        [Required] public double Latitude { get; set; }
        // this member represent the initial location's starting time
        [Required] public DateTime DateTime { get; set; }

    }
}
