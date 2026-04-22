using DesignStudio.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace DesignStudio.Server.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

      
        public DbSet<Project> Projects { get; set; }
    }
}