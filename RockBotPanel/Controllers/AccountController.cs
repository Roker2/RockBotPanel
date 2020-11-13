using RockBotPanel.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RockBotPanel.Models;
using System.Threading.Tasks;
using RockBotPanel.Helpers;

namespace RockBotPanel.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<TelegramUser> userManager;
        private readonly SignInManager<TelegramUser> signInManager;

        public AccountController(UserManager<TelegramUser> userManager,
            SignInManager<TelegramUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Copy data from RegisterViewModel to IdentityUser
                var user = new TelegramUser
                {
                    UserName = model.Email,
                    Email = model.Email
                };

                // Store user data in AspNetUsers database table
                var result = await userManager.CreateAsync(user, model.Password);

                // If user is successfully created, sign-in the user using
                // SignInManager and redirect to index action of HomeController
                if (result.Succeeded)
                {
                    await signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("index", "home");
                }

                // If there are any errors, add them to the ModelState object
                // which will be displayed by the validation summary tag helper
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("index", "home");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(
                    model.Email, model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {
                    return RedirectToAction("index", "home");
                }

                ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditUser()
        {
            var user = await userManager.GetUserAsync(User);

            string id = user.Id;

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
                return View("NotFound");
            }

            // GetClaimsAsync retunrs the list of user Claims
            var userClaims = await userManager.GetClaimsAsync(user);
            // GetRolesAsync returns the list of user Roles
            var userRoles = await userManager.GetRolesAsync(user);

            string code = RandomHelper.GenerateRandomPassword(10);
            TelegramHelper.SendString(user.TelegramId, code);

            user.LastValidationCode = code;
            var result = await userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
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
                ViewBag.ErrorMessage = $"User with Id = {model.Id} cannot be found";
                return View("NotFound");
            }
            else
            {
                if (user.LastValidationCode != model.Code)
                {
                    ViewBag.ErrorMessage = "Bad code, generated: " + user.LastValidationCode + ", you wrote: " + model.Code;
                    return View("NotFound");
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

        //TODO: Add user and chat administrator validation
        [HttpGet]
        public async Task<IActionResult> AddChatId()
        {
            var user = await userManager.GetUserAsync(User);

            string id = user.Id;

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
                return View("NotFound");
            }

            var model = new AddChatIdViewModel { };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddChatId(AddChatIdViewModel model)
        {
            var user = await userManager.GetUserAsync(User);

            if (user == null)
            {
                ViewBag.ErrorMessage = "User cannot be found";
                return View("NotFound");
            }
            else
            {
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
                    ModelState.AddModelError("", error.Description);
                }

                return View(model);
            }
        }
    }
}
