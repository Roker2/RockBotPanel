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

        public String ChatIds { get; set; } // id1|id2|id3

        public String LastValidationCode { get; set; }

        public bool CheckCode(string code) => LastValidationCode == code;
    }
}
