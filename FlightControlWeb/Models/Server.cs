using System.ComponentModel.DataAnnotations;

namespace FlightControlWeb.Models
{
    /*
     * this class represent an external server as we save it in our DBs
     */
    public class Server
    {
        [Key] public int Id { get; set; }
        [Required] public string ServerId { get; set; }
        [Required] public string ServerUrl { get; set; }
    }
}
