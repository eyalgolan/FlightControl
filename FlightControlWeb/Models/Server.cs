using System.ComponentModel.DataAnnotations;

namespace FlightControlWeb.Models
{
    public class Server
    {
        [Key] public int Id { get; set; }
        [Required] public string ServerID { get; set; }
        [Required] public string ServerURL { get; set; }
    }
}
