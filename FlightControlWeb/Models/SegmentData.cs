using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public class SegmentData
    {
        // this member represent the segment's ending longitude
        public double Longitude { get; set; }
        // this member represent the segment's ending latitude
        public double Latitude { get; set; }
        // this member represent the segment's length in seconds
        [JsonPropertyName("timespan_seconds")]
        public int TimeSpanSeconds { get; set; }
    }
}
