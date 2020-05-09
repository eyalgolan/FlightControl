using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public class Segment
    {
        [Key] public string FlightId { get; set; }
        public string Longtitude { get; set; }
        public string Latitude { get; set; }
        public string TimeSpanSeconds { get; set; }
    }
}
