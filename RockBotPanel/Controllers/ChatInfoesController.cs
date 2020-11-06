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
    public class ChatInfoesController : Controller
    {
        private readonly BotDbContext _context;

        public ChatInfoesController(BotDbContext context)
        {
            _context = context;
        }

        // GET: ChatInfoes
        public async Task<IActionResult> Index()
        {
            return View(await _context.ChatInfo.ToListAsync());
        }

        // GET: ChatInfoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chatInfo = await _context.ChatInfo
                .FirstOrDefaultAsync(m => m.Id == id);
            if (chatInfo == null)
            {
                return NotFound();
            }

            return View(chatInfo);
        }

        // GET: ChatInfoes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ChatInfoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,warns_quantity,welcome,rules,disabled_commands")] ChatInfo chatInfo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(chatInfo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(chatInfo);
        }

        // GET: ChatInfoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chatInfo = await _context.ChatInfo.FindAsync(id);
            if (chatInfo == null)
            {
                return NotFound();
            }
            return View(chatInfo);
        }

        // POST: ChatInfoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,warns_quantity,welcome,rules,disabled_commands")] ChatInfo chatInfo)
        {
            if (id != chatInfo.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(chatInfo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChatInfoExists(chatInfo.Id))
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
            return View(chatInfo);
        }

        // GET: ChatInfoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chatInfo = await _context.ChatInfo
                .FirstOrDefaultAsync(m => m.Id == id);
            if (chatInfo == null)
            {
                return NotFound();
            }

            return View(chatInfo);
        }

        // POST: ChatInfoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var chatInfo = await _context.ChatInfo.FindAsync(id);
            _context.ChatInfo.Remove(chatInfo);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ChatInfoExists(int id)
        {
            return _context.ChatInfo.Any(e => e.Id == id);
        }
    }
}
