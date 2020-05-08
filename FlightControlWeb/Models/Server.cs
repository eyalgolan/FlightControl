using System.ComponentModel.DataAnnotations;

namespace FlightControlWeb.Model
{
    public class Server
    {
        public Server(string newID, string newURL)
        {
            ServerID = newID;
            ServerURL = newURL;
        }

        [Required] public string ServerID { get; set; }

        [Required] public string ServerURL { get; set; }
    }
}