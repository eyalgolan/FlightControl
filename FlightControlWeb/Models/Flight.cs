using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public class Flight
    {
        private FlightPlan fp;
        private int id;
        private static int idCouner = 0;
        public int FlightId
        {
            get
            {
                return this.id;
            }
            set
            {
                this.id = value;
            }
        }
        public double Longtitude { get; set; }
        public double Latitude { get; set; }
        public int Passengers { get; set; }
        public String CompanyName { get; set; }
        public String DateTime { get; set; }
        public Boolean IsExternal { get; set; }
        public FlightPlan Fp { get; set; }

        public Flight (FlightPlan fpInput)
        {
            this.fp = fpInput;
            this.id = idCouner;
            idCouner++;
        }
    }
}
