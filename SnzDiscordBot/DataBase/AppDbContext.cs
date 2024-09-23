using Microsoft.EntityFrameworkCore;
using SnzDiscordBot.DataBase.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnzDiscordBot.DataBase
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<SettingsEntity> SettingsEntities { get; set; }
    }
}
