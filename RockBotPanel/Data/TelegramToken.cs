using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RockBotPanel.Data
{
    public interface ITelegramToken
    {
        string Token { get; set; }
    }
    public class TelegramToken : ITelegramToken
    {
        private string _token;
        public TelegramToken(string token)
        {
            _token = token;
        }
        public string Token
        {
            get
            {
                return _token;
            }
            set
            {
                _token = value;
            }
        }
    }
}
