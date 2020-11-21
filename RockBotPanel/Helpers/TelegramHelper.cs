using System;
using Telegram.Bot.Types;

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

        public static String GetUserName(int UserID)
        {
            Telegram.Bot.TelegramBotClient Bot = GenerateBot();
            ChatMember userInfo = Bot.GetChatMemberAsync(UserID, UserID).Result;
            return userInfo.User.Username;
        }

        public static bool IsAdmin(long ChatID, int UserID)
        {
            Telegram.Bot.TelegramBotClient Bot = GenerateBot();
            ChatMember[] chatAdmins = Bot.GetChatAdministratorsAsync(ChatID).Result;
            foreach(ChatMember admin in chatAdmins)
            {
                if (admin.User.Id == UserID)
                    return true;
            }
            return false;
        }

        public static string GetChatName(long ChatID)
        {
            Telegram.Bot.TelegramBotClient Bot = GenerateBot();
            Chat chat = Bot.GetChatAsync(ChatID).Result;
            return chat.Title;
        }
    }
}
