using Microsoft.EntityFrameworkCore;
using SnzDiscordBot.DataBase.Entities;

namespace SnzDiscordBot.DataBase;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }
    
    public DbSet<AwardEntity> Awards { get; set; }
    public DbSet<SettingsEntity> Settings { get; set; }
    public DbSet<MemberEntity> Members { get; set; }
    public DbSet<EventEntity> Events { get; set; }
    public DbSet<MemberAwardEntity> MemberAwards { get; set; }
    public DbSet<EventVoteEntity> Votes { get; set; }
}
