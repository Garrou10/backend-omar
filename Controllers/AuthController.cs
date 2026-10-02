using BackendApi.Data;
using BackendApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User loginUser)
        {

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == loginUser.Username && u.Password == loginUser.Password);

            if (user == null)
            {
                // Returnerar 401 Unauthorized om användaren inte finns eller lösenordet är fel
                return Unauthorized("Fel användarnamn eller lösenord.");
            }

            // Returnerar 200 OK om allt stämmer
            return Ok(new { message = "Inloggning lyckades!" });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User newUser)
        {
            if (await _context.Users.AnyAsync(u => u.Username == newUser.Username))
            {
                return BadRequest("Användarnamnet är redan upptaget.");
            }

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Konto skapat framgångsrikt!" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            // Letar upp uppgiften i databasen via dess ID
            var task = await _context.Tasks.FindAsync(id);
            if (task == null) return NotFound();

            // Raderar uppgiften och sparar ändringen
            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();

            return NoContent(); // Returnerar 204 No Content (Standard för lyckad Delete)
        }


    }
    
    
}