﻿using System;
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
    public class ChatinfoesController : Controller
    {
        private readonly d940mhn2jd7mllContext _context;

        public ChatinfoesController(d940mhn2jd7mllContext context)
        {
            _context = context;
        }

        // GET: Chatinfoes
        public async Task<IActionResult> Index()
        {
            return View(await _context.Chatinfo.ToListAsync());
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
            var chatinfo = await _context.Chatinfo.FindAsync(id);
            _context.Chatinfo.Remove(chatinfo);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ChatinfoExists(long id)
        {
            return _context.Chatinfo.Any(e => e.Id == id);
        }
    }
}