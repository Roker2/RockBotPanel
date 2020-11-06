using System;
using System.Collections.Generic;

namespace RockBotPanel.Models
{
    public partial class Chatinfo
    {
        public long Id { get; set; }
        public int? WarnsQuantity { get; set; }
        public string Welcome { get; set; }
        public string Rules { get; set; }
        public string DisabledCommands { get; set; }
    }
}
