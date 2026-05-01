using DesignStudio.Server.Data;
using DesignStudio.Server.Models;
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
        public async Task<ActionResult<IEnumerable<Lead>>> GetLeads()
        {

            return await _context.Leads.OrderByDescending(l => l.Id).ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Lead>> PostLead(Lead lead)
        {
           
            if (string.IsNullOrEmpty(lead.Date))
            {
                lead.Date = DateTime.Now.ToString("dd.MM.yyyy, HH:mm");
            }

            lead.Status = "new";

            _context.Leads.Add(lead);
            await _context.SaveChangesAsync();

            return Ok(lead);
        }

        [HttpPut("{id}")]
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