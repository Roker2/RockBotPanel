using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RockBotPanel.Helpers
{
    public class LayoutHelper
    {
        public static Dictionary<string, string> GetHeaderPages()
        {
            return new Dictionary<string, string>
            {
                {"Index", "Home"},
                {"Privacy", "Privacy"}
            };
        }
    }
}
