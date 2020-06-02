using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    /*
     * this class represent an object that we return in a response to
     * a GET command. it contains the data the user gets.
     */
    public class FlightPlanData
    {
        // total number of passengers in the flight
        [JsonPropertyName("passengers")]
        public int Passengers { get; set; }
        // the name of the company that operates the flight
        [JsonPropertyName("company_name")]
        public string CompanyName { get; set; }
        // the initial location of the flight (longitude, latitude and time)
        [JsonPropertyName("initial_location")]
        public InitialLocationData InitialLocation { get; set; }
        // the segments describing the flight path
        [JsonPropertyName("segments")]
        public IEnumerable<SegmentData> Segments { get; set; }
    }
}
