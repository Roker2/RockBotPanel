using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RockBotPanel.Models
{
    public class TelegramUser : IdentityUser
    {
        public int TelegramId { get; set; }
    }
}
