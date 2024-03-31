using Microsoft.EntityFrameworkCore;
namespace CFPService;

public sealed class ApplicationContext : DbContext
{
    public DbSet<Applications> Applications { get; set; } = null!;
    public DbSet<Activities> Activities { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=postgres");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        Activities report = new Activities { Activity = "Report", Description = "Доклад, 35-45 минут" };
        Activities masterclass = new Activities { Activity = "Masterclass", Description = "Мастеркласс, 1-2 часа" };
        Activities discussion = new Activities { Activity = "Discussion", Description = "Дискуссия / круглый стол, 40-50 минут" };
        modelBuilder.Entity<Activities>().HasData(report,masterclass,discussion);
    }
}