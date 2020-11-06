using System;
using System.Collections.Generic;

namespace RockBotPanel.Models
{
    public partial class Users
    {
        public long Id { get; set; }
        public int? Warns { get; set; }
        public long? Chatid { get; set; }
        public long? Userid { get; set; }
    }
}
