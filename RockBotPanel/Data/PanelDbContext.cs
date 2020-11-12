using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using RockBotPanel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RockBotPanel.Data
{
    public class PanelDbContext : IdentityDbContext
    {
        public PanelDbContext (DbContextOptions<PanelDbContext> options)
            : base(options)
        {
            //Database.EnsureCreated();
        }

        public DbSet<TelegramUser> TelegramUser { get; set; }
    }

    public class EmployeeFactory : IDesignTimeDbContextFactory<PanelDbContext>
    {
        public PanelDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<PanelDbContext>();
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=Identity;Trusted_Connection=True;MultipleActiveResultSets=true;");

            return new PanelDbContext(optionsBuilder.Options);
        }
    }
}
