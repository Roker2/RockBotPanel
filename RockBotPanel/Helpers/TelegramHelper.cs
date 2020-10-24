using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RockBotPanel.Helpers
{
    public class TelegramHelper
    {
        public static void SendCode(int UserID, int Code)
        {
            Telegram.Bot.TelegramBotClient Bot = new Telegram.Bot.TelegramBotClient("TOKEN");
            Bot.SendTextMessageAsync(UserID, Code.ToString());
        }
    }
}
