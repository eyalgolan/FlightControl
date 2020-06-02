using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FlightControlWeb.Models
{
    /*
     * this class represent a segment as we save it in our DBs
     */
    public class Segment
    {
        // this member is the inner key for any segment of flight in the DB
        [Key] public int Id { get; set; }
        // this member is the external key for any segment of flight in the DB
        // it connects between flight plan to a segment
        [ForeignKey("FlightPlan")] public int FlightPlanId { get; set; }
        // this member represent the segment's ending longitude
        [Required] public double Longitude { get; set; }
        // this member represent the segment's ending latitude
        [Required] public double Latitude { get; set; }
        // this member represent the segment's length in seconds
        [JsonPropertyName("timespan_seconds")]
        [Required] public int TimeSpanSeconds { get; set; }
        // this member represent the segment's starting time
        [Required] public DateTime StartTime { get; set; }
        // this member represent the segment's ending time
        [Required] public DateTime EndTime { get; set; }
    }
}
