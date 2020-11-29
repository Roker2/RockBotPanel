using System.Collections.Generic;

namespace RockBotPanel.ViewModels.Role
{
    public class DetailsRoleViewModel
    {
        public string Id { get; set; }
        public string RoleName { get; set; }
        public List<string> Users { get; set; } = new List<string>();
    }
}
