using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RockBotPanel.ViewModels.Users
{
    public class CreateUserViewModel
    {
        [Required]
        public int Warns { get; set; }
        [Required]
        public long Chatid { get; set; }
        [Required]
        public long Userid { get; set; }
    }
}
