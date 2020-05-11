using System;
using System.ComponentModel.DataAnnotations;

namespace FlightControlWeb.Models
{
    public class Flight
    {
        [Key] public int Id { get; set; }
        [Required] public string FlightId { get; set; }

        [Required] public bool IsExternal { get; set; }

        //public FlightPlan Fp { get; set; }

        private static string CreateId()
        {
            const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var rand = new Random();

            var id = "";
            for (var i = 0; i < 2; i++)
            {
                var j = rand.Next(0, letters.Length - 1);
                id += letters[j];
            }

            id += rand.Next(1000, 99999999).ToString();
            return id;
        }
    }
}
