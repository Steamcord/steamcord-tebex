using Microsoft.EntityFrameworkCore;

namespace Steamcord.Integrations.Tebex;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    
    public DbSet<UpdateRoleTask> Queue { get; set; }
}