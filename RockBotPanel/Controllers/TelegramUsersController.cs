using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RockBotPanel.Data;
using RockBotPanel.Models;

namespace RockBotPanel.Controllers
{
    public class TelegramUsersController : Controller
    {
        private readonly PanelDbContext _context;

        public TelegramUsersController(PanelDbContext context)
        {
            _context = context;
        }

        // GET: TelegramUsers
        public async Task<IActionResult> Index()
        {
            return View(await _context.TelegramUser.ToListAsync());
        }

        // GET: TelegramUsers/Details/5
        public async Task<IActionResult> Details(uint? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var telegramUser = await _context.TelegramUser
                .FirstOrDefaultAsync(m => m.Id == id);
            if (telegramUser == null)
            {
                return NotFound();
            }

            return View(telegramUser);
        }

        // GET: TelegramUsers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TelegramUsers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TelegramId")] TelegramUser telegramUser)
        {
            if (ModelState.IsValid)
            {
                _context.Add(telegramUser);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(telegramUser);
        }

        // GET: TelegramUsers/Edit/5
        public async Task<IActionResult> Edit(uint? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var telegramUser = await _context.TelegramUser.FindAsync(id);
            if (telegramUser == null)
            {
                return NotFound();
            }
            return View(telegramUser);
        }

        // POST: TelegramUsers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(uint id, [Bind("Id,TelegramId")] TelegramUser telegramUser)
        {
            if (id != telegramUser.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(telegramUser);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TelegramUserExists(telegramUser.Id))
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
            return View(telegramUser);
        }

        // GET: TelegramUsers/Delete/5
        public async Task<IActionResult> Delete(uint? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var telegramUser = await _context.TelegramUser
                .FirstOrDefaultAsync(m => m.Id == id);
            if (telegramUser == null)
            {
                return NotFound();
            }

            return View(telegramUser);
        }

        // POST: TelegramUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(uint id)
        {
            var telegramUser = await _context.TelegramUser.FindAsync(id);
            _context.TelegramUser.Remove(telegramUser);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TelegramUserExists(uint id)
        {
            return _context.TelegramUser.Any(e => e.Id == id);
        }
    }
}
