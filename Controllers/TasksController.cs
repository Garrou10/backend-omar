using BackendApi.Data;
using BackendApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public TasksController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskItem>>> GetTasks([FromQuery] string username)
        {
            // Om inget namn skickas med från React, returnerar vi en tom lista
            if (string.IsNullOrEmpty(username))
            {
                return new List<TaskItem>();
            }

            // Filtrerar databasen så att vi BARA returnerar uppgifter som tillhör den inloggade användaren
            return await _context.Tasks
                .Where(t => t.Username == username)
                .ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<TaskItem>> PostTask(TaskItem task)
        {
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetTasks), new { id = task.Id }, task);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutTask(int id, TaskItem task)
        {
            if (id != task.Id) return BadRequest();

            _context.Entry(task).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("{id}/image")]
        public async Task<IActionResult> UploadImage(int id, IFormFile file)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null) return NotFound();

            if (file == null || file.Length == 0) return BadRequest("Ingen fil valdes.");

            // Använd IWebHostEnvironment för att garanterat hitta rätt wwwroot-mapp
            var webRootPath = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var uploadsFolder = Path.Combine(webRootPath, "uploads");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // Ta bort den befintliga bilden från disken om användaren byter bild
            if (!string.IsNullOrEmpty(task.ImageUrl))
            {
                var oldFileName = Path.GetFileName(task.ImageUrl);
                var oldFilePath = Path.Combine(uploadsFolder, oldFileName);
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
            }

            // Skapa det nya filnamnet med korrekt filändelse (.jpg, .png etc.)
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Spara den relativa webbsökvägen i databasen
            task.ImageUrl = $"/uploads/{fileName}";
            await _context.SaveChangesAsync();

            return Ok(task);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null) return NotFound();

            // Ta bort den tillhörande bilden från disken när själva uppgiften raderas
            if (!string.IsNullOrEmpty(task.ImageUrl))
            {
                var webRootPath = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                var fileName = Path.GetFileName(task.ImageUrl);
                var filePath = Path.Combine(webRootPath, "uploads", fileName);

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}