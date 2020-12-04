using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        private readonly IEmailMessanger _emailMessenger;
        private readonly ILogger<AdminPanelController> _logger;

        public AdminPanelController(UserManager<TelegramUser> userManager,
            IEmailMessanger emailMessenger,
            ILogger<AdminPanelController> logger)
        {
            this.userManager = userManager;
            _emailMessenger = emailMessenger;
            _logger = logger;
            _logger.LogDebug("Admin Panel controller constructor");
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
                if(user.Email != null)
                {
                    userList.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                    {
                        Text = $"{user.Email} ({user.UserName})",
                        Value = user.Email,
                        Selected = false
                    });
                }
            }
            model.UserList = userList.OrderBy(u => u.Value).ToList();
            return View(model);
        }

        [HttpPost]
        public IActionResult SendMail(EmailMessageViewModel model)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }
            EmailMessage msg = new EmailMessage
            {
                From = model.From,
                Subject = model.Subject,
                Content = model.Content
            };
            if(model.SelectedEmails == null)
            {
                _logger.LogError("User didn't choose emails");
                ModelState.AddModelError("", "Select emails");
                return View(model);
            }
            foreach(var email in model.SelectedEmails)
            {
                msg.ToAddresses.Add(email);
            }
            _emailMessenger.SendMsg(msg);
            return RedirectToAction("Index", "AdminPanel");
        }

        [HttpGet]
        public IActionResult UsersList()
        {
            List<TelegramUser> users = userManager.Users.ToList();
            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
                _logger.LogError($"User with Id = {id} cannot be found");
                return View("NotFound");
            }
            else
            {
                var result = await userManager.DeleteAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction("UsersList");
                }

                foreach (var error in result.Errors)
                {
                    _logger.LogError(error.Description);
                    ModelState.AddModelError("", error.Description);
                }

                return View("UsersList");
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);

            if (user == null)
            {
                //_logger.LogError($"User with Id = {id} cannot be found");
                ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
                _logger.LogError($"User with Id = {id} cannot be found");
                return View("NotFound");
            }

            // GetClaimsAsync retunrs the list of user Claims
            var userClaims = await userManager.GetClaimsAsync(user);
            // GetRolesAsync returns the list of user Roles
            var userRoles = await userManager.GetRolesAsync(user);
            /*user.GenerateValidationCode();
            var result = await userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                //_logger.LogError("Can not save validation code");
                ViewBag.ErrorMessage = "Can not save validation code";
                return View("NotFound");
            }*/

            var model = new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                TelegramId = user.TelegramId
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            var user = await userManager.FindByIdAsync(model.Id);

            if (user == null)
            {
                _logger.LogError($"User with Id = {model.Id} cannot be found");
                ViewBag.ErrorMessage = $"User with Id = {model.Id} cannot be found";
                return View("NotFound");
            }
            else
            {
                /*if (!user.CheckCode(model.Code))
                {
                    //_logger.LogError($"Bad code, {user.UserName} wrote: {model.Code}");
                    ModelState.AddModelError(string.Empty, $"Bad code, you wrote: {model.Code}");
                    return View(model);
                }*/
                user.TelegramId = model.TelegramId;
                user.Email = model.Email;
                user.UserName = model.UserName;

                var result = await userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction("UsersList", "AdminPanel");
                }

                foreach (var error in result.Errors)
                {
                    _logger.LogError(error.Description);
                    ModelState.AddModelError("", error.Description);
                }

                return View(model);
            }
        }
    }
}
