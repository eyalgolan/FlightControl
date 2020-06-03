using System;
using System.Text.Json.Serialization;

namespace FlightControlWeb.Models
{
    /*
     * this class represent an object that we return in a response to
     * a GET command. it contains the data the user gets.
     */
    public class InitialLocationData
    {
        // this member represent the initial location's longitude
        public double Longitude { get; set; }
        // this member represent the initial location's latitude
        public double Latitude { get; set; }
        // this member represent the initial location's starting time
        [JsonPropertyName("date_time")]
        public DateTime DateTime { get; set; }

    }
}
