using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public class FlightData
    {
        [JsonPropertyName("flight_id")]
        public string flight_id { get; set; }
        [JsonPropertyName("is_external")]
        public bool IsExternal { get; set; }
        [JsonPropertyName("longitude")]
        public double longitude { get; set; }
        [JsonPropertyName("latitude")]
        public double latitude { get; set; }
        [JsonPropertyName("passengers")]
        public int passengers { get; set; }
        [JsonPropertyName("company_name")]
        public string company_name { get; set; }
        [JsonPropertyName("date_time")]
        public DateTime date_time { get; set; }
    }
}
