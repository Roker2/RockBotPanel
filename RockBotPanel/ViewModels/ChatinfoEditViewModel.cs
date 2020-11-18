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
        public ChatinfoEditViewModel()
        { }
        public ChatinfoEditViewModel(Chatinfo chatinfo)
        {
            Id = chatinfo.Id;
            WarnsQuantity = chatinfo.WarnsQuantity.Value;
            Welcome = chatinfo.Welcome;
            Rules = chatinfo.Rules;
            DisabledCommands = chatinfo.DisabledCommands;
        }
        public long Id { get; set; }

        public int WarnsQuantity { get; set; }

        public string Welcome { get; set; }

        public string Rules { get; set; }

        public string DisabledCommands { get; set; }
    }
}
