using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RockBotPanel.Models
{
    public class EmailMessage
    {
        public List<string> ToAddresses { get; set; } = new List<string>();
        public string Subject { get; set; }
        public string Content { get; set; }
    }
}
