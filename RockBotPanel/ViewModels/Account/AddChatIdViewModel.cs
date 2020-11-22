using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RockBotPanel.ViewModels.Account
{
    public class AddChatIdViewModel
    {

        [Required]
        public long ChatId { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Code { get; set; }
    }
}
