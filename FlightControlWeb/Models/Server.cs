using System.ComponentModel.DataAnnotations;

namespace FlightControlWeb.Model
{
    public class Server
    {
        public Server(int newID, string newURL)
        {
            ServerID = newID;
            ServerURL = newURL;
        }

        [Required] public int ServerID { get; set; }

        [Required] public string ServerURL { get; set; }
    }
}