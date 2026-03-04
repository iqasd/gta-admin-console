using GtaAdminReportsApp.Models;
using Microsoft.EntityFrameworkCore;

namespace GtaAdminReportsApp.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Report> Reports => Set<Report>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Report>(entity =>
        {
            entity.ToTable("reports");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.PlayerName).HasMaxLength(64).IsRequired();
            entity.Property(x => x.ViolationType).HasMaxLength(64).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(1024).IsRequired();
            entity.Property(x => x.Status).HasMaxLength(32).IsRequired();
            entity.Property(x => x.TargetAdminClass).HasConversion<int>();
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Username).HasMaxLength(64).IsRequired();
            entity.Property(x => x.AdminClass).HasConversion<int>();
        });
    }
}
