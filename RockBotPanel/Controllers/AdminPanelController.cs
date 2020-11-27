using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RockBotPanel.Models;
using RockBotPanel.Services;
using RockBotPanel.ViewModels.AdminPanel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RockBotPanel.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminPanelController : Controller
    {
        private readonly UserManager<TelegramUser> userManager;
        private readonly IEmailMessenger _emailMessenger;

        public AdminPanelController(UserManager<TelegramUser> userManager,
            IEmailMessenger emailMessenger)
        {
            this.userManager = userManager;
            _emailMessenger = emailMessenger;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult SendMail()
        {
            EmailMessageViewModel model = new EmailMessageViewModel();
            List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> userList = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();
            foreach (var user in userManager.Users)
            {
                userList.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Text = $"{user.Email} ({user.UserName})",
                    Value = user.Email,
                    Selected = false
                });
            }
            model.UserList = userList.OrderBy(u => u.Value).ToList();
            return View(model);
        }

        [HttpPost]
        public IActionResult SendMail(EmailMessageViewModel model)
        {
            EmailMessage msg = new EmailMessage
            {
                From = model.From,
                Subject = model.Subject,
                Content = model.Content
            };
            foreach(var email in model.SelectedEmails)
            {
                msg.ToAddresses.Add(email);
            }
            _emailMessenger.SendMsg(msg);
            return RedirectToAction("Index", "AdminPanel");
        }
    }
}
