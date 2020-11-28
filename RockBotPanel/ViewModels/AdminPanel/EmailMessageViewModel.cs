using RockBotPanel.Models;
using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RockBotPanel.ViewModels.AdminPanel
{
    public class EmailMessageViewModel
    {
        [Required]
        public string From { get; set; } = "Rock Bot Panel";
        [Required]
        public string Subject { get; set; }
        [Required]
        public string Content { get; set; }
        public IEnumerable<string> SelectedEmails { get; set; }
        public IEnumerable<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> UserList { get; set; }
    }
}
