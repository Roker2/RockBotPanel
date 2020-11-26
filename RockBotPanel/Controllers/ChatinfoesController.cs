using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RockBotPanel.Data;
using RockBotPanel.Models;
using RockBotPanel.ViewModels.Chatinfoes;

namespace RockBotPanel.Controllers
{
    [Authorize]
    public class ChatinfoesController : Controller
    {
        private readonly UserManager<TelegramUser> userManager;
        private readonly d940mhn2jd7mllContext context;
        private readonly ILogger<AccountController> _logger;

        public ChatinfoesController(UserManager<TelegramUser> userManager,
            d940mhn2jd7mllContext context,
            ILogger<AccountController> logger)
        {
            this.context = context;
            this.userManager = userManager;
            _logger = logger;
            _logger.LogDebug("Chatinfoes controller constructor");
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
            List<Chatinfo> chats = await context.Chatinfo.ToListAsync();
            List<Chatinfo> filteredChats = new List<Chatinfo>();

            foreach(Chatinfo chat in chats)
            {
                string chatId = chat.Id.ToString();
                if (ids.Find(x => x == chatId) != null)
                {
                    filteredChats.Add(chat);
                }
            }

            if (filteredChats.Count == 0)
                _logger.LogWarning("filteredChats is empty");

            return View(filteredChats);
        }

        // GET: Chatinfoes/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                _logger.LogError("id is null");
                return NotFound();
            }

            var chatinfo = await context.Chatinfo
                .FirstOrDefaultAsync(m => m.Id == id);
            if (chatinfo == null)
            {
                _logger.LogError("chatinfo is null");
                return NotFound();
            }

            return View(chatinfo);
        }

        // GET: Chatinfoes/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                _logger.LogError("id is null");
                return NotFound();
            }

            TelegramUser user = await userManager.GetUserAsync(User);
            if (user.ChatIds == null)
            {
                _logger.LogError("chatinfo is null");
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
                _logger.LogError("Chat " + id.ToString() + " is not added in user chats");
                return View("NotFound");
            }

            var chatinfo = await context.Chatinfo.FindAsync(id);
            if (chatinfo == null)
            {
                return NotFound();
            }
            return View(new ChatinfoEditViewModel(chatinfo));
        }

        // POST: Chatinfoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, ChatinfoEditViewModel model)
        {
            Chatinfo chatinfo = model.ToChatinfo();
            if (id != chatinfo.Id)
            {
                _logger.LogError("id and chatinfo.Id are different");
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    context.Update(chatinfo);
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChatinfoExists(chatinfo.Id))
                    {
                        _logger.LogError($"chatinfo {chatinfo.Id} does not exist");
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(new ChatinfoEditViewModel(chatinfo));
        }

        // GET: Chatinfoes/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                _logger.LogError("id is null");
                return NotFound();
            }

            var chatinfo = await context.Chatinfo
                .FirstOrDefaultAsync(m => m.Id == id);
            if (chatinfo == null)
            {
                _logger.LogError("chatinfo is null");
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
                _logger.LogError(error.Description);
                ModelState.AddModelError("", error.Description);
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ChatinfoExists(long id)
        {
            return context.Chatinfo.Any(e => e.Id == id);
        }
    }
}
