using System;
using System.ComponentModel.DataAnnotations;

namespace FlightControlWeb.Models
{
    /*
     * this class represent a flight as we save it in our DBs
     */
    public class Flight
    {
        // internal flight id for our own purposes in the program
        [Key] public int Id { get; set; }
        // external flight id, used for interactions with external servers
        // and external HTML commands
        [Required] public string FlightId { get; set; }
        // this member indicates the source of a given flight
        public string OriginServer { get; set; }
        // this member is true when the flight is external, false otherwise.
        public bool IsExternal { get; set; }
    }
}
