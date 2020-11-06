using Microsoft.EntityFrameworkCore;
using RockBotPanel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RockBotPanel.Data
{
    public class BotDbContext : DbContext
    {
        public BotDbContext(DbContextOptions<PanelDbContext> options)
            : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=usersdb;Username=postgres;Password=password");
        }

        public DbSet<RockBotPanel.Models.ChatInfo> ChatInfo { get; set; }
    }
}
