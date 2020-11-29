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

        public AccountController(UserManager<TelegramUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<TelegramUser> signInManager,
            d940mhn2jd7mllContext context,
            ILogger<AccountController> logger,
            ITelegramService telegramService)
        {
            _context = context;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.signInManager = signInManager;
            _telegramService = telegramService;
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
                return View("NotFound");
            }
            ViewBag.ShowFullView = true;
            return View(model);
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Copy data from RegisterViewModel to IdentityUser
                var user = new TelegramUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    TelegramId = model.TelegramId
                };

                // Store user data in AspNetUsers database table
                var result = await userManager.CreateAsync(user, model.Password);

                // If user is successfully created, sign-in the user using
                // SignInManager and redirect to index action of HomeController
                if (result.Succeeded)
                {
                    //Set role "User" as default role
                    result = await userManager.AddToRoleAsync(user, "User");
                    if (result.Succeeded)
                    {
                        await signInManager.SignInAsync(user, isPersistent: false);
                        return RedirectToAction("index", "home");
                    }
                }

                // If there are any errors, add them to the ModelState object
                // which will be displayed by the validation summary tag helper
                foreach (var error in result.Errors)
                {
                    _logger.LogError(error.Description);
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            _logger.LogInformation("RegisterViewModel model is invalid");
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("index", "home");
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            //don't show validation code
            ViewBag.ShowFullView = false;
            return View();
        }

        /*
         * In the first time show only Email
         * In next times show full view, if email is right
         */
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            TelegramUser user;
            if (ModelState.IsValid)
            {
                user = await userManager.FindByEmailAsync(model.Email);
                if(user == null)
                {
                    _logger.LogError($"There is no such user, {model.Email}");
                    ModelState.AddModelError("", "There is no such user with this Email");
                    ViewBag.ShowFullView = false;
                    return View(model);
                }
                //send validation code
                if(model.ValidationCode == null && model.Password == null)
                {
                    //Generate, send and save validation code
                    string code = RandomHelper.GenerateRandomPassword(10);
                    _telegramService.SendString(user.TelegramId, "Validation code: " + code);
                    user.LastValidationCode = code;
                    var result = await userManager.UpdateAsync(user);

                    if (!result.Succeeded)
                    {
                        _logger.LogCritical("Can not save validation code");
                        ModelState.AddModelError("", "Can not save validation code");
                        return View("NotFound");
                    }
                    //Okay, show validation code and password
                    ViewBag.ShowFullView = true;
                    return View(model);
                }
                try
                {
                    //if password is null, site will show "Password is empty"
                    if (model.Password == null)
                    {
                        _logger.LogError("User didn't write password");
                        ModelState.AddModelError("", "Password is empty");
                        return await LoginCorrupt(model, user);
                    }
                    //check validation code
                    if (!user.CheckCode(model.ValidationCode))
                    {
                        _logger.LogError("Validatin code is not correct");
                        ModelState.AddModelError("", "Validatin code is not correct");
                        return await LoginCorrupt(model, user);
                    }
                    var result = await signInManager.PasswordSignInAsync(
                        model.Email, model.Password, model.RememberMe, false);

                    if (result.Succeeded)
                    {
                        return RedirectToAction("index", "home");
                    }

                    ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
                    _logger.LogInformation("Invalid Login Attempt");
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);
                    ViewBag.ErrorMessage = e;
                    return View("NotFound");
                }
            }

            _logger.LogInformation("LoginViewModel model is invalid");
            //if Email is null, show full view
            if(model.Email != null)
            //if(ModelState.IsValid)
            {
                user = await userManager.FindByEmailAsync(model.Email);
                ViewBag.ShowFullView = true;
                return await LoginCorrupt(model, user);
            }
            //if Email is not correct, don't show full view
            ViewBag.ShowFullView = false;
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditUser()
        {
            var user = await userManager.GetUserAsync(User);

            string id = user.Id;

            if (user == null)
            {
                _logger.LogError($"User with Id = {id} cannot be found");
                ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
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
                return View("NotFound");
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
        public async Task<IActionResult> AddChatId()
        {
            var user = await userManager.GetUserAsync(User);

            string id = user.Id;

            if (user == null)
            {
                _logger.LogError($"User with Id = {id} cannot be found");
                ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
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
                return View("NotFound");
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
                return View("NotFound");
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
