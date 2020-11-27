using RockBotPanel.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace RockBotPanel.Services
{
    public interface ITelegramService
    {
        void SendString(int UserID, string str);
        string GetUserName(int UserID);
        string GetUserName(int UserID, long ChatID);
        bool IsAdmin(long ChatID, int UserID);
        string GetChatName(long ChatID);
    }
    public class TelegramService : ITelegramService
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly Telegram.Bot.TelegramBotClient Bot;

        public TelegramService(ITelegramToken telegramToken)
        {
            Bot = new Telegram.Bot.TelegramBotClient(telegramToken.Token);
        }

        public void SendString(int UserID, string str)
        {
            Bot.SendTextMessageAsync(UserID, str);
        }

        public String GetUserName(int UserID)
        {
            ChatMember userInfo = Bot.GetChatMemberAsync(UserID, UserID).Result;
            return userInfo.User.Username;
        }

        public String GetUserName(int UserID, long ChatID)
        {
            try
            {
                ChatMember userInfo = Bot.GetChatMemberAsync(ChatID, UserID).Result;
                return userInfo.User.Username;
            }
            catch (AggregateException e)
            {
                logger.Warn($"User {UserID} is not in chat {ChatID}, error: {e.Message}");
                return "User is't in chat";
            }
        }

        public bool IsAdmin(long ChatID, int UserID)
        {
            ChatMember[] chatAdmins = Bot.GetChatAdministratorsAsync(ChatID).Result;
            foreach (ChatMember admin in chatAdmins)
            {
                if (admin.User.Id == UserID)
                    return true;
            }
            return false;
        }

        public string GetChatName(long ChatID)
        {
            Chat chat = Bot.GetChatAsync(ChatID).Result;
            return chat.Title;
        }
    }
}
