using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlightControlWeb.Models
{
    /*
     * this class represent a flight plan as we save it in our DBs
     */
    public class FlightPlan
    {
        // internal flight plan id for our own purposes in the program
        [Key] public int Id { get; set; }
        // external flight id, used for interactions with external servers
        // and external HTML commands
        [ForeignKey("Flight")] public string FlightId { get; set; }
        // total number of passengers in the flight
        [Required] public int Passengers { get; set; }
        // the name of the company that operates the flight
        [Required] public string CompanyName { get; set; }
        // this member is true when the flight is external, false otherwise.
        [Required] public bool IsExternal { get; set; }
        // the date in which the flight ends
        public DateTime EndTime { get; set; }
        // this member indicates the source of a given flight
        public string OriginServer { get; set; }
    }
}
