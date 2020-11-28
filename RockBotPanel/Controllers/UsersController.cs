using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RockBotPanel.Data;
using RockBotPanel.Helpers;
using RockBotPanel.Models;
using RockBotPanel.Services;
using RockBotPanel.ViewModels.Users;

namespace RockBotPanel.Controllers
{
    //Table name in the database is "Users". When I did table, I didn't know about rules for table names. Sorry.
    [Authorize]
    public class UsersController : Controller
    {
        private readonly UserManager<TelegramUser> userManager;
        private readonly d940mhn2jd7mllContext context;
        private readonly ILogger<AccountController> _logger;
        private readonly ITelegramService _telegramService;

        public UsersController(UserManager<TelegramUser> userManager,
            d940mhn2jd7mllContext context,
            ILogger<AccountController> logger,
            ITelegramService telegramService)
        {
            this.userManager = userManager;
            this.context = context;
            _telegramService = telegramService;
            _logger = logger;
            _logger.LogDebug("Users controller constructor");
        }

        // GET: Users
        public async Task<IActionResult> Index(long? id)
        {
            if (id == null)
            {
                _logger.LogError("id is null");
                return NotFound();
            }

            List<Users> allUsers = await context.Users.ToListAsync();
            List<Users> filteredUsers = new List<Users>();
            foreach(Users user in allUsers)
            {
                if (user.Chatid == id.Value)
                    filteredUsers.Add(user);
            }
            ViewBag.ChatName = _telegramService.GetChatName(id.Value);

            //Get max warns quantuty
            var chatinfo = await context.Chatinfo
                .FirstOrDefaultAsync(m => m.Id == id);
            ViewBag.MaxWarnsQuantity = chatinfo.WarnsQuantity;

            if(filteredUsers.Count == 0)
                _logger.LogWarning("filteredUsers is empty");

            //Add service for page
            ViewBag.telegramService = _telegramService;

            ViewBag.Chatid = id;
            return View(filteredUsers);
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                _logger.LogError("id is null");
                return NotFound();
            }

            var users = await context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (users == null)
            {
                _logger.LogError($"users with ID = {id} is null");
                return NotFound();
            }

            //Add service for page
            ViewBag.telegramService = _telegramService;

            return View(users);
        }

        // GET: Users/Create
        //id is chat ID
        public IActionResult Create(long? chatid)
        {
            if (chatid == null)
            {
                _logger.LogError("id is null");
                return NotFound();
            }

            var user = userManager.GetUserAsync(User).Result;
            bool isAdmin = _telegramService.IsAdmin(chatid.Value, user.TelegramId);
            if (!isAdmin)
            {
                _logger.LogError($"User is not admin in {_telegramService.GetChatName(chatid.Value)}");
                ViewBag.ErrorMessage = $"You are not admin in {_telegramService.GetChatName(chatid.Value)}";
                return View("NotFound");
            }
            CreateUserViewModel model = new CreateUserViewModel { Chatid = chatid.Value };

            return View(model);
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                Users users = new Users {
                    Id = model.Userid + model.Chatid,
                    Chatid = model.Chatid,
                    Userid = model.Userid,
                    Warns = model.Warns
                };
                context.Add(users);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { id = users.Chatid });
            }
            _logger.LogInformation("Users.CreateUserViewModel model is not valid");
            return View(model);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                _logger.LogError("id is null");
                return NotFound();
            }

            var users = await context.Users.FindAsync(id);
            if (users == null)
            {
                _logger.LogError($"users with ID = {id} is null");
                return NotFound();
            }

            var user = await userManager.GetUserAsync(User);
            bool isAdmin = _telegramService.IsAdmin(users.Chatid.Value, user.TelegramId);
            if (!isAdmin)
            {
                _logger.LogError($"User is not admin in {_telegramService.GetChatName(users.Chatid.Value)}");
                ViewBag.ErrorMessage = $"You are not admin in {_telegramService.GetChatName(users.Chatid.Value)}";
                return View("NotFound");
            }
            return View(users);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Id,Warns,Chatid,Userid")] Users users)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    context.Update(users);
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsersExists(users.Id))
                    {
                        ViewBag.ErrorMessage = "User does not exist";
                        _logger.LogError("User does not exist");
                        return View("NotFound");
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index), new { id = users.Chatid });
            }
            _logger.LogInformation("Users model is invalid");
            return View(users);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var users = await context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (users == null)
            {
                return NotFound();
            }

            var user = await userManager.GetUserAsync(User);
            bool isAdmin = _telegramService.IsAdmin(users.Chatid.Value, user.TelegramId);
            if (!isAdmin)
            {
                _logger.LogError($"User is not admin in {_telegramService.GetChatName(users.Chatid.Value)}");
                ViewBag.ErrorMessage = $"You are not admin in {_telegramService.GetChatName(users.Chatid.Value)}";
                return View("NotFound");
            }

            //Add service for page
            ViewBag.telegramService = _telegramService;

            return View(users);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var users = await context.Users.FindAsync(id);
            context.Users.Remove(users);
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { id = users.Chatid });
        }

        private bool UsersExists(long id)
        {
            return context.Users.Any(e => e.Id == id);
        }
    }
}
