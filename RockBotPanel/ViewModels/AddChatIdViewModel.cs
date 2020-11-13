using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RockBotPanel.ViewModels
{
    public class AddChatIdViewModel
    {

        [Required]
        public long ChatId { get; set; }
    }
}
