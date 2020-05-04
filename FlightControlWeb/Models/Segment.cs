using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public class Segment
    {
        private int longtitude;
        private int latitude;
        private string timespanSeconds;

        public Segment(int longtitudeInput, int latitudeInput, string timespanSecondsInput)
        {
            this.longtitude = longtitudeInput;
            this.latitude = latitudeInput;
            this.timespanSeconds = timespanSecondsInput;
        }
    }
}
