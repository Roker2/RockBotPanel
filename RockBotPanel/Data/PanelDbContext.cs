using Microsoft.EntityFrameworkCore;
using RockBotPanel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RockBotPanel.Data
{
    public class PanelDbContext : DbContext
    {
        public PanelDbContext (DbContextOptions<PanelDbContext> options)
            : base(options)
        {

        }

        public DbSet<TelegramUser> TelegramUser { get; set; }
    }
}
