using System;
using System.Collections.Generic;

namespace RockBotPanel.Models
{
    public partial class Allusers
    {
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        private string username;
        public long Id { get; set; }
        public string Username
        {
            get
            {
                if (username == "")
                    return "No username";
                return username;
            }
            set => username = value;
        }
    }
}
