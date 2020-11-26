using System;
using System.Collections.Generic;

namespace RockBotPanel.Models
{
    public partial class Allusers
    {
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        private string username;
        public long Id { get; set; }
        public string Username
        {
            get
            {
                if (username == "")
                {
                    logger.Debug($"User {Id} does not have username");
                    return "No username";
                }
                return username;
            }
            set => username = value;
        }
    }
}
