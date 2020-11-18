using RockBotPanel.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RockBotPanel.ViewModels
{
    public class ChatinfoEditViewModel
    {
        public ChatinfoEditViewModel(Chatinfo chatinfo)
        {
            Id = chatinfo.Id;
            WarnsQuantity = chatinfo.WarnsQuantity.Value;
            Welcome = chatinfo.Welcome;
            Rules = chatinfo.Rules;
            List<string> commandsList = chatinfo.DisabledCommands.Split(" ").ToList();
            if (commandsList != null)
            {
                Disable_randomal = commandsList.Find(x => x == "randomal") != null;
                Disable_welcome = commandsList.Find(x => x == "welcome") != null;
                Disable_randomsmmq = commandsList.Find(x => x == "randomsmmq") != null;
            }
        }

        private string GenerateDisabledCommands()
        {
            string DisabledCommands = null;
            if (Disable_randomal)
                DisabledCommands = "randomal";
            if (Disable_welcome)
            {
                if (DisabledCommands != null)
                    DisabledCommands += " welcome";
                else
                    DisabledCommands = "welcome";
            }
            if (Disable_randomsmmq)
            {
                if (DisabledCommands != null)
                    DisabledCommands += " randomsmmq";
                else
                    DisabledCommands = "randomsmmq";
            }
            return DisabledCommands;
        }

        public Chatinfo ToChatinfo()
        {
            return new Chatinfo
            {
                Id = this.Id,
                DisabledCommands = GenerateDisabledCommands(),
                Rules = this.Rules,
                WarnsQuantity = this.WarnsQuantity,
                Welcome = this.Welcome
            };
        }
        public long Id { get; set; }

        public int WarnsQuantity { get; set; }

        public string Welcome { get; set; }

        public string Rules { get; set; }

        public bool Disable_randomal { get; set; }

        public bool Disable_welcome { get; set; }

        public bool Disable_randomsmmq { get; set; }
    }
}
