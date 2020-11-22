using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RockBotPanel.ViewModels.Account
{
    public class EditUserViewModel
    {
        public string Id { get; set; }

        [Required]
        public int TelegramId { get; set; }

        [Required]
        public string UserName { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Code { get; set; }
    }
}
