using DesignStudio.Server.Data;
using DesignStudio.Server.Models;
using Microsoft.AspNetCore.Authorization; // ДОБАВЛЕНО
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DesignStudio.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProjectsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        // Открыто: галерея проектов доступна всем
        public async Task<ActionResult<IEnumerable<Project>>> GetProjects()
        {
            return await _context.Projects.OrderByDescending(p => p.Id).ToListAsync();
        }

        [HttpGet("{id}")]
        // Открыто: просмотр одного проекта доступен всем
        public async Task<ActionResult<Project>> GetProject(int id)
        {
            var project = await _context.Projects.FindAsync(id);

            if (project == null)
            {
                return NotFound("Проект не найден");
            }

            return project;
        }

        [HttpPost]
        [Authorize] // Защищено: добавлять может только админ
        public async Task<ActionResult<Project>> PostProject(Project project)
        {
            if (project.Images == null)
            {
                project.Images = new List<string>();
            }

            project.Images = await SaveImagesToFolderAsync(project.Images);

            project.CreatedAt = DateTime.UtcNow;
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            return Ok(project);
        }

        [HttpDelete("{id}")]
        [Authorize] // Защищено: удалять может только админ
        public async Task<IActionResult> DeleteProject(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("{id}")]
        [Authorize] // Защищено: изменять может только админ
        public async Task<IActionResult> PutProject(int id, Project project)
        {
            if (id != project.Id)
            {
                return BadRequest("ID проекта не совпадает");
            }

            if (project.Images == null)
            {
                project.Images = new List<string>();
            }

            project.Images = await SaveImagesToFolderAsync(project.Images);
            _context.Entry(project).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Projects.Any(e => e.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        private async Task<List<string>> SaveImagesToFolderAsync(List<string> base64Images)
        {
            var savedUrls = new List<string>();

            if (base64Images == null || base64Images.Count == 0)
            {
                return savedUrls;
            }

            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "projects");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            foreach (var base64 in base64Images)
            {
                if (string.IsNullOrWhiteSpace(base64)) continue;

                if (base64.StartsWith("http") || base64.StartsWith("/"))
                {
                    savedUrls.Add(base64);
                    continue;
                }

                try
                {
                    var base64Data = base64.Contains(",") ? base64.Split(',')[1] : base64;
                    var bytes = Convert.FromBase64String(base64Data);

                    var fileName = Guid.NewGuid().ToString() + ".jpg";
                    var filePath = Path.Combine(folderPath, fileName);

                    await System.IO.File.WriteAllBytesAsync(filePath, bytes);

                    savedUrls.Add($"https://localhost:7002/images/projects/{fileName}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка сохранения картинки: " + ex.Message);
                }
            }
            return savedUrls;
        }
    }
}