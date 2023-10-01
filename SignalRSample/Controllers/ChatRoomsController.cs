using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SignalRSample.Data;
using SignalRSample.Models;

namespace SignalRSample.Controllers
{
    [ApiController]
    public class ChatRoomsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ChatRoomsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/ChatRooms
        [HttpGet]
        [Route("/[controller]/GetChatRoom")]
        public async Task<ActionResult<IEnumerable<ChatRoom>>> GetChatRoom()
        {
          if (_context.ChatRooms == null)
          {
              return NotFound();
          }
            return await _context.ChatRooms.ToListAsync();
        }

        // GET: api/ChatRooms
        [HttpGet]
        [Route("/[controller]/GetChatUser")]
        public async Task<ActionResult<object>> GetChatUser()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var users = await _context.Users.Where(u=> u.Id !=userId).Select(u=> new {u.Id, u.UserName}).ToListAsync() ;
            if (users == null)
            {
                return NotFound();
            }

            return users;
        }

        // POST: api/ChatRooms
        [HttpPost]
        [Route("/[controller]/PostChatRoom")]
        public async Task<ActionResult<ChatRoom>> PostChatRoom(ChatRoom chatRoom)
        {
          if (_context.ChatRooms == null || string.IsNullOrEmpty(chatRoom.Name))
          {
              return Problem("Entity set 'ApplicationDbContext.ChatRooms'  is null.");
          }
            _context.ChatRooms.Add(chatRoom);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetChatRoom", new { id = chatRoom.Id }, chatRoom);
        }

        // DELETE: api/ChatRooms/5
        [HttpDelete("{id}")]
        [Route("/[controller]/DeleteChatRooms/{id}")]
        public async Task<IActionResult> DeleteChatRoom(int id)
        {
            if (_context.ChatRooms == null)
            {
                return NotFound();
            }
            var chatRoom = await _context.ChatRooms.FindAsync(id);
            if (chatRoom == null)
            {
                return NotFound();
            }

            _context.ChatRooms.Remove(chatRoom);
            await _context.SaveChangesAsync();

            var room = await _context.ChatRooms.FirstOrDefaultAsync();
            return Ok(new {deleted = id, selected = (room == null ? 0  : room.Id)});
        }

    }
}
