using Microsoft.EntityFrameworkCore;
using VolHub.Mvc.Models;

namespace VolHub.Mvc.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> o) : base(o) { }

    public DbSet<AppUserProfile> Profiles => Set<AppUserProfile>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<AppUserProfile>()
          .HasIndex(p => p.Username)
          .IsUnique();
        mb.Entity<AppUserProfile>()
          .HasIndex(p => p.Email)
          .IsUnique();
    }
}
