using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
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

        }
    }
}
