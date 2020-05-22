using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FlightControlWeb.Models;

namespace FlightControlWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServersController : ControllerBase
    {
        private readonly ServerContext _context;

        public ServersController(ServerContext context)
        {
            _context = context;
        }

        // GET: api/Servers
        /* 
        * This method is our implementation of HTTP GET command.
        * once the user asks for the entire servers we return a list of all the servers
        * that we have active flights from in our DBs.
        */
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Server>>> GetAllServers()
        {
            return await _context.serverItems.ToListAsync();
        }

        // POST: api/Servers
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.

        /* 
        * This method is our implementation of HTTP POST command.
        * when we want to add a server to our program we use it.
        * if the server is already exist in our DB we return conflict
        */
        [HttpPost]
        public async Task<ActionResult<Server>> PostServer(Server server)
        {
            _context.serverItems.Add(server);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ServerExists(server.ServerID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetAllServers", new { id = server.ServerID }, server);
        }

        // DELETE: api/Servers/5

        /* 
        * This method is our implementation of HTTP DELETE command.
        * when we want to delete a server from our DB we check if it exists.
        * if it doesn't we return not found. otherwise it removes the server
        * from our DB
        */
        [HttpDelete("{id}")]
        public async Task<ActionResult<Server>> DeleteServer(string serverId)
        {
            var server = await _context.serverItems.Where(x => x.ServerID == serverId).FirstOrDefaultAsync();
            if (server == null)
            {
                return NotFound();
            }

            _context.serverItems.Remove(server);
            await _context.SaveChangesAsync();

            return server;
        }

        /* 
        * This method returns true if the given server exist in our DB, false otherwise.
        */
        private bool ServerExists(string id)
        {
            return _context.serverItems.Any(e => e.ServerID == id);
        }
    }
}