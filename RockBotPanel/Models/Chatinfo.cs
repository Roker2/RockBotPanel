using RockBotPanel.Helpers;
using System;
using System.Collections.Generic;

namespace RockBotPanel.Models
{
    public partial class Chatinfo
    {
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        private int? _WarnsQuantity;
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        private string _Welcome;
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        private string _Rules;
        public long Id { get; set; }
        public int? WarnsQuantity
        {
            get
            {
                if (!_WarnsQuantity.HasValue)
                    return DefaultsHelper.DefaultWarnsQuantity;
                return _WarnsQuantity;
            }
            set => _WarnsQuantity = value.GetValueOrDefault();
        }
        public string Welcome
        {
            get
            {
                if (_Welcome == null)
                    return DefaultsHelper.DefaultWelcome;
                return _Welcome;
            }
            set => _Welcome = value;
        }
        public string Rules
        {
            get
            {
                if (_Rules == null)
                    return DefaultsHelper.DefaultRules;
                return _Rules;
            }
            set => _Rules = value;
        }
        public string DisabledCommands { get; set; }

        public string GetChatName()
        {
            return TelegramHelper.GetChatName(Id);
        }
    }
}
