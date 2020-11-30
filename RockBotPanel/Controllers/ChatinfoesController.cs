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
using RockBotPanel.Services;
using RockBotPanel.ViewModels.Chatinfoes;

namespace RockBotPanel.Controllers
{
    [Authorize]
    public class ChatinfoesController : Controller
    {
        private readonly UserManager<TelegramUser> userManager;
        private readonly d940mhn2jd7mllContext context;
        private readonly ILogger<AccountController> _logger;
        private readonly ITelegramService _telegramService;

        public ChatinfoesController(UserManager<TelegramUser> userManager,
            d940mhn2jd7mllContext context,
            ILogger<AccountController> logger,
            ITelegramService telegramService)
        {
            this.context = context;
            this.userManager = userManager;
            _telegramService = telegramService;
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
            List<Chatinfo> chats = await context.Chatinfo.ToListAsync();
            List<Chatinfo> filteredChats = new List<Chatinfo>();

            foreach(Chatinfo chat in chats)
            {
                if (_telegramService.IsAdmin(chat.Id, user.TelegramId))
                {
                    filteredChats.Add(chat);
                }
            }

            if (filteredChats.Count == 0)
                _logger.LogWarning("filteredChats is empty");

            //Add service for page
            ViewBag.telegramService = _telegramService;

            return View(filteredChats);
        }

        // GET: Chatinfoes/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                _logger.LogError("id is null");
                ViewBag.ErrorMessage = "Id is empty";
                return View("SiteError");
            }

            var chatinfo = await context.Chatinfo
                .FirstOrDefaultAsync(m => m.Id == id);
            if (chatinfo == null)
            {
                _logger.LogError($"chatinfo with ID = {id} is null");
                ViewBag.ErrorMessage = $"Chat with ID = {id} cannot be found";
                return View("NotFound");
            }

            //Add service for page
            ViewBag.telegramService = _telegramService;

            return View(chatinfo);
        }

        // GET: Chatinfoes/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                _logger.LogError("id is null");
                ViewBag.ErrorMessage = "Id is empty";
                return View("SiteError");
            }

            TelegramUser user = await userManager.GetUserAsync(User);

            if (!_telegramService.IsAdmin(id.Value, user.TelegramId))
            {
                _logger.LogInformation($"User is not admin in {_telegramService.GetChatName(id.Value)}");
                ViewBag.ErrorMessage = $"You are not admin in {_telegramService.GetChatName(id.Value)}";
                return View("SiteError");
            }

            var chatinfo = await context.Chatinfo.FindAsync(id);
            if (chatinfo == null)
            {
                _logger.LogError($"chatinfo with ID = {id} is null");
                ViewBag.ErrorMessage = $"Chat with ID = {id} cannot be found";
                return View("NotFound");
            }
            return View(new ChatinfoEditViewModel(chatinfo));
        }

        // POST: Chatinfoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ChatinfoEditViewModel model)
        {
            Chatinfo chatinfo = model.ToChatinfo();
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
                        ViewBag.ErrorMessage = $"Chat with ID = {chatinfo.Id} cannot be found";
                        return View("NotFound");
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            _logger.LogInformation("chatinfo model is not valid");
            return View(model);
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
                _logger.LogError($"chatinfo with ID = {id} is null");
                ViewBag.ErrorMessage = $"Chat with ID = {id} cannot be found";
                return View("NotFound");
            }

            //Add service for page
            ViewBag.telegramService = _telegramService;

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
