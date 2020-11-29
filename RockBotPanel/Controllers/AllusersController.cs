using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RockBotPanel.Data;
using RockBotPanel.Models;

namespace RockBotPanel.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AllusersController : Controller
    {
        private readonly d940mhn2jd7mllContext _context;
        private readonly ILogger<AccountController> _logger;

        public AllusersController(d940mhn2jd7mllContext context,
            ILogger<AccountController> logger)
        {
            _context = context;
            _logger = logger;
            _logger.LogDebug("Allusers controller constructor");
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
                _logger.LogError("id is null");
                ViewBag.ErrorMessage = "Id is empty";
                return View("SiteError");
            }

            var allusers = await _context.Allusers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (allusers == null)
            {
                _logger.LogError($"allusers with ID = {id} is null");
                ViewBag.ErrorMessage = $"User with ID = {id} cannot be found";
                return View("NotFound");
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
                _logger.LogError("id is null");
                ViewBag.ErrorMessage = "Id is empty";
                return View("SiteError");
            }

            var allusers = await _context.Allusers.FindAsync(id);
            if (allusers == null)
            {
                _logger.LogError($"allusers with ID = {id} is null");
                ViewBag.ErrorMessage = $"User with ID = {id} cannot be found";
                return View("NotFound");
            }
            return View(allusers);
        }

        // POST: Allusers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Id,Username")] Allusers allusers)
        {
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
                        _logger.LogError($"allusers {allusers.Id} does not exist");
                        ViewBag.ErrorMessage = $"User with ID = {allusers.Id} cannot be found";
                        return View("NotFound");
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            _logger.LogInformation("allusers model is not valid");
            return View(allusers);
        }

        // GET: Allusers/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                _logger.LogError("id is null");
                ViewBag.ErrorMessage = "Id is empty";
                return View("SiteError");
            }

            var allusers = await _context.Allusers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (allusers == null)
            {
                _logger.LogError($"allusers with ID = {id} is null");
                ViewBag.ErrorMessage = $"User with ID = {allusers.Id} cannot be found";
                return View("NotFound");
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
