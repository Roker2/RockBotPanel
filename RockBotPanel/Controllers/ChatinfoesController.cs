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
using RockBotPanel.Models;

namespace RockBotPanel.Controllers
{
    [Authorize]
    public class ChatinfoesController : Controller
    {
        private readonly UserManager<TelegramUser> userManager;
        private readonly d940mhn2jd7mllContext _context;
        private readonly PanelDbContext _panelContext;

        public ChatinfoesController(UserManager<TelegramUser> userManager, d940mhn2jd7mllContext context, PanelDbContext panelContext)
        {
            _context = context;
            _panelContext = panelContext;
            this.userManager = userManager;
        }

        // GET: Chatinfoes
        public async Task<IActionResult> Index()
        {
            TelegramUser user = await userManager.GetUserAsync(User);
            if(user.ChatIds == null)
            {
                return View(new List<Chatinfo>());
            }
            List<string> ids = user.ChatIds.Split("|").ToList();
            List<Chatinfo> chats = await _context.Chatinfo.ToListAsync();
            List<Chatinfo> filteredChats = new List<Chatinfo>();

            foreach(Chatinfo chat in chats)
            {
                string chatId = chat.Id.ToString();
                if (ids.Find(x => x == chatId) != null)
                {
                    filteredChats.Add(chat);
                }
            }

            return View(filteredChats);
        }

        // GET: Chatinfoes/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chatinfo = await _context.Chatinfo
                .FirstOrDefaultAsync(m => m.Id == id);
            if (chatinfo == null)
            {
                return NotFound();
            }

            return View(chatinfo);
        }

        // GET: Chatinfoes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Chatinfoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,WarnsQuantity,Welcome,Rules,DisabledCommands")] Chatinfo chatinfo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(chatinfo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(chatinfo);
        }

        // GET: Chatinfoes/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            TelegramUser user = await userManager.GetUserAsync(User);
            if (user.ChatIds == null)
            {
                return View(new List<Chatinfo>());
            }
            List<string> ids = user.ChatIds.Split("|").ToList();
            bool found = false;

            foreach (string userChatsId in ids)
            {
                if(userChatsId == id.ToString())
                {
                    found = true;
                    break;
                }
            }

            if(!found)
            {
                ViewBag.ErrorMessage = "Chat " + id.ToString() + " is not added in your chats";
                return View("NotFound");
            }

            var chatinfo = await _context.Chatinfo.FindAsync(id);
            if (chatinfo == null)
            {
                return NotFound();
            }
            return View(chatinfo);
        }

        // POST: Chatinfoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,WarnsQuantity,Welcome,Rules,DisabledCommands")] Chatinfo chatinfo)
        {
            if (id != chatinfo.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(chatinfo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChatinfoExists(chatinfo.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(chatinfo);
        }

        // GET: Chatinfoes/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chatinfo = await _context.Chatinfo
                .FirstOrDefaultAsync(m => m.Id == id);
            if (chatinfo == null)
            {
                return NotFound();
            }

            return View(chatinfo);
        }

        // POST: Chatinfoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            TelegramUser user = await userManager.GetUserAsync(User);
            List<string> ids = user.ChatIds.Split("|").ToList();
            ids.Remove(id.ToString());
            switch (ids.Count)
            {
                //user deleted all chats
                case 0:
                    user.ChatIds = null;
                    break;
                //user saved only 1 chat
                case 1:
                    user.ChatIds = ids[0];
                    break;
                //user saved more then 1 chats
                default:
                    user.ChatIds = ids[0];
                    foreach (string chatId in ids)
                    {
                        if (chatId == ids[0])
                            continue;
                        user.ChatIds += "|" + chatId;
                    }
                    break;
            }

            var result = await userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ChatinfoExists(long id)
        {
            return _context.Chatinfo.Any(e => e.Id == id);
        }
    }
}
