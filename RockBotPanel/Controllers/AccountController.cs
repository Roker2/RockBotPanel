using RockBotPanel.ViewModels.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RockBotPanel.Models;
using System.Threading.Tasks;
using RockBotPanel.Helpers;
using Microsoft.AspNetCore.Authorization;
using RockBotPanel.Data;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.Extensions.Logging;
using RockBotPanel.Services;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace RockBotPanel.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<TelegramUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly SignInManager<TelegramUser> signInManager;
        private readonly d940mhn2jd7mllContext _context;
        private readonly ILogger<AccountController> _logger;
        private readonly ITelegramService _telegramService;
        private readonly ITelegramToken _telegramToken;

        public AccountController(UserManager<TelegramUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<TelegramUser> signInManager,
            d940mhn2jd7mllContext context,
            ILogger<AccountController> logger,
            ITelegramService telegramService,
            ITelegramToken telegramToken)
        {
            _context = context;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.signInManager = signInManager;
            _telegramService = telegramService;
            _telegramToken = telegramToken;
            _logger = logger;
            _logger.LogDebug("Account controller constructor");
        }

        async Task<IActionResult> LoginCorrupt(LoginViewModel model, TelegramUser user)
        {
            //Generate, send and save validation code
            string code = RandomHelper.GenerateRandomPassword(10);
            _telegramService.SendString(user.TelegramId, "Validation code: " + code);
            user.LastValidationCode = code;
            var result = await userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                _logger.LogError("Can not save validation code");
                ModelState.AddModelError("", "Can not save validation code");
                return View("SiteError");
            }
            ViewBag.ShowFullView = true;
            return View(model);
        }

        private string HmacSha256Digest(string message, string token)
        {
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] keyBytes;
            using (SHA256 sha256 = SHA256.Create())
            {
                keyBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(token));
            }
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            HMACSHA256 cryptographer = new HMACSHA256(keyBytes);

            byte[] bytes = cryptographer.ComputeHash(messageBytes);

            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("index", "home");
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Login(int? id, string first_name,
            string last_name, string username, string photo_url, long? auth_date, string hash)
        {
            if (id == null)
            {
                ViewBag.ErrorMessage = "id is null";
                return View("SiteError");
            }
            if (first_name == null)
            {
                ViewBag.ErrorMessage = "first_name is null";
                return View("SiteError");
            }
            if (photo_url == null)
            {
                ViewBag.ErrorMessage = "photo_url is null";
                return View("SiteError");
            }
            if (auth_date == null)
            {
                ViewBag.ErrorMessage = "auth_date is null";
                return View("SiteError");
            }
            if (hash == null)
            {
                ViewBag.ErrorMessage = "hash is null";
                return View("SiteError");
            }
            string data = $"auth_date={auth_date.Value}\nfirst_name={first_name}\nid={id.Value}\nlast_name={last_name}\nphoto_url={photo_url}\nusername={username}";
            string siteHash = HmacSha256Digest(data, _telegramToken.Token);
            if(siteHash != hash)
            {
                ViewBag.ErrorMessage = "Hash is not correct";
                _logger.LogInformation("User can not login, enable debug, if you want to see authorization values");
                _logger.LogDebug(data);
                _logger.LogDebug($"siteHash={siteHash}");
                return View("SiteError");
            }

            List<TelegramUser> users = userManager.Users.ToList();
            TelegramUser currentUser = null;
            foreach(TelegramUser user in users)
            {
                if(user.TelegramId == id)
                {
                    currentUser = user;
                    break;
                }
            }
            if(currentUser == null)
            {
                currentUser = new TelegramUser { TelegramId = id.Value };
                if (username != null)
                    currentUser.UserName = username;
                else
                    currentUser.UserName = first_name;
                var result = await userManager.CreateAsync(currentUser);
                if (!result.Succeeded)
                {
                    _logger.LogInformation($"Can not register user with Telegram ID {id}");
                    ModelState.AddModelError("", $"Can not register you");
                    return View("SiteError");
                }
            }
            await signInManager.SignInAsync(currentUser, true);
            return RedirectToAction("index", "home");
        }


        [HttpGet]
        public async Task<IActionResult> EditUser()
        {
            var user = await userManager.GetUserAsync(User);

            if (user == null)
            {
                _logger.LogError($"User cannot be found");
                ViewBag.ErrorMessage = $"User cannot be found";
                return View("NotFound");
            }

            //Generate, send and save validation code
            string code = RandomHelper.GenerateRandomPassword(10);
            _telegramService.SendString(user.TelegramId, "Validation code: " + code);
            user.LastValidationCode = code;
            var result = await userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                _logger.LogError("Can not save validation code");
                ViewBag.ErrorMessage = "Can not save validation code";
                return View("SiteError");
            }

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
            var user = await userManager.GetUserAsync(User);

            if (user == null)
            {
                _logger.LogError($"User with Id = {model.Id} cannot be found");
                ViewBag.ErrorMessage = $"User with Id = {model.Id} cannot be found";
                return View("NotFound");
            }
            else
            {
                if (!user.CheckCode(model.Code))
                {
                    _logger.LogError($"Bad code, {user.UserName} wrote: {model.Code}");
                    ModelState.AddModelError(string.Empty, $"Bad code, you wrote: {model.Code}");
                    return View(model);
                }
                user.TelegramId = model.TelegramId;
                user.Email = model.Email;
                user.UserName = model.UserName;

                var result = await userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction("index", "home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> DeleteUser()
        {
            TelegramUser user = await userManager.GetUserAsync(User);
            //Generate, send and save validation code
            string code = RandomHelper.GenerateRandomPassword(10);
            _telegramService.SendString(user.TelegramId, "Validation code: " + code);
            user.LastValidationCode = code;
            var result = await userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                _logger.LogError("Can not save validation code");
                ViewBag.ErrorMessage = "Can not save validation code";
                return View("SiteError");
            }

            return View(new DeleteUserViewModel());
        }

        //code is validation code
        [HttpPost, ActionName("DeleteUser")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(DeleteUserViewModel model)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }

            TelegramUser user = await userManager.GetUserAsync(User);

            if (user == null)
            {
                ViewBag.ErrorMessage = "User cannot be found and delete account";
                _logger.LogError("User cannot be found and delete account");
                return View("NotFound");
            }
            else
            {
                if(!user.CheckCode(model.ValidationCode))
                {
                    _logger.LogError($"Bad code, {user.UserName} wrote: {model.ValidationCode}");
                    ModelState.AddModelError("ValidationCode", $"Bad code, you wrote: {model.ValidationCode}");

                    //Regenerate, send and save validation code
                    string code = RandomHelper.GenerateRandomPassword(10);
                    _telegramService.SendString(user.TelegramId, "Validation code: " + code);
                    user.LastValidationCode = code;
                    IdentityResult identityResult = await userManager.UpdateAsync(user);

                    if (!identityResult.Succeeded)
                    {
                        _logger.LogError("Can not save validation code");
                        ViewBag.ErrorMessage = "Can not save validation code";
                        return View("SiteError");
                    }

                    return View(model);
                }
                //logout and delete
                await signInManager.SignOutAsync();
                var result = await userManager.DeleteAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction("index", "home");
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
        public async Task<IActionResult> AddChatId()
        {
            var user = await userManager.GetUserAsync(User);

            if (user == null)
            {
                _logger.LogError($"User cannot be found");
                ViewBag.ErrorMessage = $"User cannot be found";
                return View("NotFound");
            }

            //Generate, send and save validation code
            string code = RandomHelper.GenerateRandomPassword(10);
            _telegramService.SendString(user.TelegramId, "Validation code: " + code);
            user.LastValidationCode = code;
            var result = await userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                _logger.LogError("Can not save validation code");
                ViewBag.ErrorMessage = "Can not save validation code";
                return View("SiteError");
            }

            var model = new AddChatIdViewModel();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddChatId(AddChatIdViewModel model)
        {
            var user = await userManager.GetUserAsync(User);

            if (user == null)
            {
                _logger.LogError("User cannot be found");
                ViewBag.ErrorMessage = "User cannot be found";
                return View("NotFound");
            }

            if(!user.CheckCode(model.Code))
            {
                ModelState.AddModelError(string.Empty, $"Bad code, you wrote: {model.Code}");
                _logger.LogInformation($"Bad code, {user.UserName} wrote: {model.Code}");
                return View(model);
            }

            bool isAdmin;

            try
            {
                isAdmin = _telegramService.IsAdmin(model.ChatId, user.TelegramId);
            }
            catch(Exception e)
            {
                _logger.LogError(e.Message);
                _logger.LogError($"Bot didn't find chat {model.ChatId}");
                ViewBag.ErrorMessage = $"Bot didn't find chat {model.ChatId}. If you didn't add bot to chat, do it.";
                return View("NotFound");
            }

            if (!isAdmin)
            {
                _logger.LogError($"{user.UserName} is not admin in {_telegramService.GetChatName(model.ChatId)}");
                ViewBag.ErrorMessage = $"You are not admin in {_telegramService.GetChatName(model.ChatId)}";
                return View("SiteError");
            }
            else
            {
                Chatinfo chat = await _context.Chatinfo.FirstOrDefaultAsync(m => m.Id == model.ChatId);
                if(chat == null)
                {
                    Chatinfo chatinfo = new Chatinfo
                    {
                        Id = model.ChatId
                    };
                    _context.Add(chatinfo);
                    await _context.SaveChangesAsync();
                }
                if (user.ChatIds != null)
                    user.ChatIds += "|" + model.ChatId.ToString();
                else
                    user.ChatIds = model.ChatId.ToString();

                var result = await userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction("index", "Chatinfoes");
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
