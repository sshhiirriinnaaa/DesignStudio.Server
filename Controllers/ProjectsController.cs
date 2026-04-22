using DesignStudio.Server.Data;
using DesignStudio.Server.Models;
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
        public async Task<ActionResult<IEnumerable<Project>>> GetProjects()
        {
         
            return await _context.Projects.ToListAsync();
        }

    
        [HttpPost]
        public async Task<ActionResult<Project>> PostProject(Project project)
        {
           
            _context.Projects.Add(project);
         
            await _context.SaveChangesAsync();

         
            return Ok(project);
        }
    }
}