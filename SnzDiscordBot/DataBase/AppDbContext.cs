using Microsoft.EntityFrameworkCore;
using SnzDiscordBot.DataBase.Entities;

namespace SnzDiscordBot.DataBase;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SettingsEntity>()
            .HasIndex(x => x.GuildId)
            .IsUnique();
        
        base.OnModelCreating(modelBuilder);
    }

    public DbSet<SettingsEntity> SettingsEntities { get; set; }
}
