using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DB_First_Handson.Models;

namespace DB_First_Handson.Controllers
{
    public class TravelExperiencesController : Controller
    {
        private readonly TravelBuddyDbContext _context;

        public TravelExperiencesController(TravelBuddyDbContext context)
        {
            _context = context;
        }

        // GET: TravelExperiences
        public async Task<IActionResult> Index()
        {
            return View(await _context.TravelExperiences.ToListAsync());
        }

        // GET: TravelExperiences/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var travelExperience = await _context.TravelExperiences
                .FirstOrDefaultAsync(m => m.ExperienceId == id);
            if (travelExperience == null)
            {
                return NotFound();
            }

            return View(travelExperience);
        }

        // GET: TravelExperiences/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TravelExperiences/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ExperienceId,Destination,TravelDate,Rating,IsSoloTravel")] TravelExperience travelExperience)
        {
            if (ModelState.IsValid)
            {
                _context.Add(travelExperience);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(travelExperience);
        }

        // GET: TravelExperiences/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var travelExperience = await _context.TravelExperiences.FindAsync(id);
            if (travelExperience == null)
            {
                return NotFound();
            }
            return View(travelExperience);
        }

        // POST: TravelExperiences/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ExperienceId,Destination,TravelDate,Rating,IsSoloTravel")] TravelExperience travelExperience)
        {
            if (id != travelExperience.ExperienceId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(travelExperience);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TravelExperienceExists(travelExperience.ExperienceId))
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
            return View(travelExperience);
        }

        // GET: TravelExperiences/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var travelExperience = await _context.TravelExperiences
                .FirstOrDefaultAsync(m => m.ExperienceId == id);
            if (travelExperience == null)
            {
                return NotFound();
            }

            return View(travelExperience);
        }

        // POST: TravelExperiences/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var travelExperience = await _context.TravelExperiences.FindAsync(id);
            if (travelExperience != null)
            {
                _context.TravelExperiences.Remove(travelExperience);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TravelExperienceExists(int id)
        {
            return _context.TravelExperiences.Any(e => e.ExperienceId == id);
        }
    }
}
