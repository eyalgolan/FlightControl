using System.ComponentModel.DataAnnotations;

namespace FlightControlWeb.Models
{
    public class Segment
    {
        [Key] public int Id { get; set; }
        public string Longtitude { get; set; }
        public string Latitude { get; set; }
        public string TimeSpanSeconds { get; set; }
    }
}
