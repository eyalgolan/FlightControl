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
    public class FlightData
    {
        // the "external" id of the flight
        [JsonPropertyName("flight_id")]
        public string FlightID { get; set; }
        // the initial longitude of the flight
        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }
        // the initial latitude of the flight
        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }
        // total number of passengers in the flight
        [JsonPropertyName("passengers")]
        public int Passengers { get; set; }
        // the name of the company that operates the flight
        [JsonPropertyName("company_name")]
        public string CompanyName { get; set; }
        // the date and time that the flight departures
        [JsonPropertyName("date_time")]
        public DateTime CurrDateTime { get; set; }
        // an indication for the flight and is it from
        // internal program or external server
        [JsonPropertyName("is_external")]
        public bool IsExternal { get; set; }
    }
}
