using System;

namespace RockBotPanel.Helpers
{
    public static class RandomHelper
    {
        public static String GenerateRandomPassword(int length)
        {
            Random random = new Random(Environment.TickCount);
            String chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789", pswd = "";
            for (int i = 0; i < length; i++)
                pswd += chars[random.Next(chars.Length)];
            return pswd;
        }
    }
}
