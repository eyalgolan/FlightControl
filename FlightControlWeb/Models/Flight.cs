using System;
using System.ComponentModel.DataAnnotations;

namespace FlightControlWeb.Models
{
    public class Flight
    {
        [Key] public int Id { get; set; }
        [Required] public string FlightId { get; set; }
        [Required] public bool IsExternal { get; set; }
        public double CurrentLongitude { get; set; }
        public double CurrentLatitude { get; set; }
        public string CompanyName { get; set; }
        public DateTime CurrentDateTime { get; set; }
    }
}
