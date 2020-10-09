using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SmoothieCity.Models;

namespace SmoothieCity.Controllers
{
    public class SmoothiesController : Controller
    {
        private readonly SmoothieCityContext _context;

        public SmoothiesController(SmoothieCityContext context)
        {
            _context = context;
        }

        // GET: Smoothies
        public async Task<IActionResult> Index()
        {
            return View(await _context.Smoothies.ToListAsync());
        }

        // GET: Smoothies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var smoothies = await _context.Smoothies
                .FirstOrDefaultAsync(m => m.SmoothieId == id);
            if (smoothies == null)
            {
                return NotFound();
            }

            return View(smoothies);
        }

        // GET: Smoothies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Smoothies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SmoothieId,SmoothieName,SmoothieCalories,SmoothiePrice,SmoothieImage")] Smoothies smoothies)
        {
            if (ModelState.IsValid)
            {
                _context.Add(smoothies);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(smoothies);
        }

        // GET: Smoothies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var smoothies = await _context.Smoothies.FindAsync(id);
            if (smoothies == null)
            {
                return NotFound();
            }
            return View(smoothies);
        }

        // POST: Smoothies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SmoothieId,SmoothieName,SmoothieCalories,SmoothiePrice,SmoothieImage")] Smoothies smoothies)
        {
            if (id != smoothies.SmoothieId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(smoothies);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SmoothiesExists(smoothies.SmoothieId))
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
            return View(smoothies);
        }

        // GET: Smoothies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var smoothies = await _context.Smoothies
                .FirstOrDefaultAsync(m => m.SmoothieId == id);
            if (smoothies == null)
            {
                return NotFound();
            }

            return View(smoothies);
        }

        // POST: Smoothies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var smoothies = await _context.Smoothies.FindAsync(id);
            _context.Smoothies.Remove(smoothies);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SmoothiesExists(int id)
        {
            return _context.Smoothies.Any(e => e.SmoothieId == id);
        }
    }
}
