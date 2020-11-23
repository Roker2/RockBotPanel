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
    public class AllusersController : Controller
    {
        private readonly d940mhn2jd7mllContext _context;

        public AllusersController(d940mhn2jd7mllContext context)
        {
            _context = context;
        }

        // GET: Allusers
        public async Task<IActionResult> Index()
        {
            return View(await _context.Allusers.ToListAsync());
        }

        // GET: Allusers/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var allusers = await _context.Allusers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (allusers == null)
            {
                return NotFound();
            }

            return View(allusers);
        }

        // GET: Allusers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Allusers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Username")] Allusers allusers)
        {
            if (ModelState.IsValid)
            {
                _context.Add(allusers);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(allusers);
        }

        // GET: Allusers/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var allusers = await _context.Allusers.FindAsync(id);
            if (allusers == null)
            {
                return NotFound();
            }
            return View(allusers);
        }

        // POST: Allusers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Username")] Allusers allusers)
        {
            if (id != allusers.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(allusers);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AllusersExists(allusers.Id))
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
            return View(allusers);
        }

        // GET: Allusers/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var allusers = await _context.Allusers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (allusers == null)
            {
                return NotFound();
            }

            return View(allusers);
        }

        // POST: Allusers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var allusers = await _context.Allusers.FindAsync(id);
            _context.Allusers.Remove(allusers);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AllusersExists(long id)
        {
            return _context.Allusers.Any(e => e.Id == id);
        }
    }
}
