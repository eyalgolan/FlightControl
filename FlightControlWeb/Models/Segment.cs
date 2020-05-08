using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public class Segment
    {
        private string longtitude;
        private string latitude;
        private string timespanSeconds;

        public Segment(string longtitudeInput, string latitudeInput, string timespanSecondsInput)
        {
            this.longtitude = longtitudeInput;
            this.latitude = latitudeInput;
            this.timespanSeconds = timespanSecondsInput;
        }

        public string Longtitude { get; set; }
        public string Latitude { get; set; }
        public string TimeSpanSeconds { get; set; }
    }
}
