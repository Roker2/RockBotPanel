using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RockBotPanel.Helpers
{
    public class TelegramHelper
    {
        protected static Telegram.Bot.TelegramBotClient GenerateBot()
        {
            return new Telegram.Bot.TelegramBotClient("TOKEN");
        }

        public static void SendCode(int UserID, int Code)
        {
            Telegram.Bot.TelegramBotClient Bot = GenerateBot();
            Bot.SendTextMessageAsync(UserID, Code.ToString());
        }

        public static void SendString(int UserID, string str)
        {
            Telegram.Bot.TelegramBotClient Bot = GenerateBot();
            Bot.SendTextMessageAsync(UserID, str);
        }
    }
}
