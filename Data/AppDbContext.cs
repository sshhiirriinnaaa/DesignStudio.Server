using DesignStudio.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace DesignStudio.Server.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Эта строчка создаст таблицу Projects в MySQL
        public DbSet<Project> Projects { get; set; }
    }
}