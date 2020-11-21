using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RockBotPanel.Data;
using RockBotPanel.Helpers;
using RockBotPanel.Models;
using RockBotPanel.ViewModels;

namespace RockBotPanel.Controllers
{
    //Table name in the database is "Users". When I did table, I didn't know about rules for table names. Sorry.
    [Authorize]
    public class UsersController : Controller
    {
        private readonly UserManager<TelegramUser> userManager;
        private readonly d940mhn2jd7mllContext _context;

        public UsersController(UserManager<TelegramUser> userManager, d940mhn2jd7mllContext context)
        {
            this.userManager = userManager;
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            List<Users> allUsers = await _context.Users.ToListAsync();
            List<Users> filteredUsers = new List<Users>();
            foreach(Users user in allUsers)
            {
                if (user.Chatid == id.Value)
                    filteredUsers.Add(user);
            }
            ViewBag.ChatName = Helpers.TelegramHelper.GetChatName(id.Value);

            //Get max warns quantuty
            var chatinfo = await _context.Chatinfo
                .FirstOrDefaultAsync(m => m.Id == id);
            ViewBag.MaxWarnsQuantity = chatinfo.WarnsQuantity;

            ViewBag.Chatid = id;
            return View(filteredUsers);
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var users = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (users == null)
            {
                return NotFound();
            }

            return View(users);
        }

        // GET: Users/Create
        //id is chat ID
        public IActionResult Create(long? chatid)
        {
            if (chatid == null)
            {
                return NotFound();
            }

            var user = userManager.GetUserAsync(User).Result;
            bool isAdmin = user.IsAdmin(chatid.Value);
            if (!isAdmin)
            {
                ViewBag.ErrorMessage = "You are not admin in " + TelegramHelper.GetChatName(chatid.Value);
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
                _context.Add(users);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { id = users.Chatid });
            }
            return View(model);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var users = await _context.Users.FindAsync(id);
            if (users == null)
            {
                return NotFound();
            }

            var user = await userManager.GetUserAsync(User);
            bool isAdmin = user.IsAdmin(users.Chatid.Value);
            if (!isAdmin)
            {
                ViewBag.ErrorMessage = "You are not admin in " + TelegramHelper.GetChatName(users.Chatid.Value);
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
                    _context.Update(users);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsersExists(users.Id))
                    {
                        ViewBag.ErrorMessage = "User does not exist";
                        return View("NotFound");
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index), new { id = users.Chatid });
            }
            return View(users);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var users = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (users == null)
            {
                return NotFound();
            }

            var user = await userManager.GetUserAsync(User);
            bool isAdmin = user.IsAdmin(users.Chatid.Value);
            if (!isAdmin)
            {
                ViewBag.ErrorMessage = "You are not admin in " + TelegramHelper.GetChatName(users.Chatid.Value);
                return View("NotFound");
            }

            return View(users);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var users = await _context.Users.FindAsync(id);
            _context.Users.Remove(users);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { id = users.Chatid });
        }

        private bool UsersExists(long id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
