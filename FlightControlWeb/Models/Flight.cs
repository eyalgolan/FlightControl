using System;
using System.ComponentModel.DataAnnotations;

namespace FlightControlWeb.Models
{
    public class Flight
    {
        [Key] public int Id { get; set; }
        [Required] public string FlightId { get; set; }
        public string OriginServer { get; set; }
        public bool IsExternal { get; set; }
    }
}
