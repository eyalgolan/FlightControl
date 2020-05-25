using System.ComponentModel.DataAnnotations;

namespace FlightControlWeb.Models
{
    public class Server
    {
        [Key] public int Id { get; set; }
        [Required] public string ServerId { get; set; }
        [Required] public string ServerUrl { get; set; }
    }
}
