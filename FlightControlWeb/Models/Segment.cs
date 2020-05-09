using System.ComponentModel.DataAnnotations;

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
