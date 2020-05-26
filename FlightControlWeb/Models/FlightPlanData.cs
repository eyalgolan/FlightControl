using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public class FlightPlanData
    {
        [JsonPropertyName("passengers")]
        public int Passengers { get; set; }
        [JsonPropertyName("company_name")]
        public string CompanyName { get; set; }
        [JsonPropertyName("initial_location")]
        public InitialLocation InitialLocation { get; set; }
        [JsonPropertyName("segments")]
        public IEnumerable<Segment> Segments { get; set; }
    }
}
