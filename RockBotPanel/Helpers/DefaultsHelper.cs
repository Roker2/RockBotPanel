using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RockBotPanel.Helpers
{
    public class DefaultsHelper
    {
        public static int DefaultWarnsQuantity() => 5;

        public static string DefaultRules() => "Правила не установлены!";

        public static string DefaultWelcome() => "Добро пожаловать, {firstName}!";
    }
}
