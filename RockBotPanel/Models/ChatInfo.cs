using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RockBotPanel.Models
{
    public class ChatInfo
    {
        public int Id { get; set; }
        public int warns_quantity { get; set; }
        public string welcome { get; set; }
        public string rules { get; set; }
        public string disabled_commands { get; set; }
    }
}
