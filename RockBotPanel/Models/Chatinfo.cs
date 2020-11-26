using RockBotPanel.Helpers;
using System;
using System.Collections.Generic;

namespace RockBotPanel.Models
{
    public partial class Chatinfo
    {
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
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
                {
                    logger.Debug($"Chat {Id} use default warns quantity");
                    return DefaultsHelper.DefaultWarnsQuantity;
                }
                return _WarnsQuantity;
            }
            set => _WarnsQuantity = value.GetValueOrDefault();
        }
        public string Welcome
        {
            get
            {
                if (_Welcome == null)
                {
                    logger.Debug($"Chat {Id} use default welcome");
                    return DefaultsHelper.DefaultWelcome;
                }
                return _Welcome;
            }
            set => _Welcome = value;
        }
        public string Rules
        {
            get
            {
                if (_Rules == null)
                {
                    logger.Debug($"Chat {Id} use default rules");
                    return DefaultsHelper.DefaultRules;
                }
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
