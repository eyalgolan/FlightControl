using System.ComponentModel.DataAnnotations;

namespace FlightControlWeb.Models
{
    /*
     * this class represent an external server as we save it in our DBs
     */
    public class Server
    {
        // this member is the inner key for any server in the DB
        [Key] public int Id { get; set; }
        // this member is the id for any server in the DB
        [Required] public string ServerId { get; set; }
        // this member is the url of any server in the DB
        [Required] public string ServerUrl { get; set; }
    }
}
