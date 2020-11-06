using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RockBotPanel.Models
{
    public class TelegramUser
    {
        [Key]
        public uint Id { get; set; }
        public int TelegramId { get; set; }
        public IdentityUser User { get; set; }
    }
}
