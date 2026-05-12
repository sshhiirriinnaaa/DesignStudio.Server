using DesignStudio.Server.Data;
using DesignStudio.Server.Models;
using Microsoft.AspNetCore.Authorization; // ДОБАВЛЕНО
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace DesignStudio.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeadsController : ControllerBase
    {
        private readonly AppDbContext _context;
        public LeadsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize] // Защищено: только админ видит список заявок
        public async Task<ActionResult<IEnumerable<Lead>>> GetLeads()
        {
            return await _context.Leads.OrderByDescending(l => l.Id).ToListAsync();
        }

        [HttpPost]
        // Открыто: любой посетитель сайта может отправить заявку
        public async Task<ActionResult<Lead>> PostLead(Lead lead)
        {
            lead.CreatedAt = DateTime.UtcNow;
            lead.Status = "new";

            _context.Leads.Add(lead);
            await _context.SaveChangesAsync();

            return Ok(lead);
        }

        [HttpPut("{id}")]
        [Authorize] // Защищено: только админ меняет статус
        public async Task<IActionResult> PutLead(int id, Lead lead)
        {
            if (id != lead.Id)
            {
                return BadRequest();
            }

            _context.Entry(lead).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}