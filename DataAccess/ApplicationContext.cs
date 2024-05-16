using Domain;
using Microsoft.EntityFrameworkCore;

namespace DataAccess;

public sealed class ApplicationContext : DbContext
{
    public DbSet<Applications> Applications { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=postgres");
    }
}