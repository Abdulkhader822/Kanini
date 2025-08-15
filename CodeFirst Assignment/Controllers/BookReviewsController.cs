using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CodeFirst_Assignment.Models;

namespace CodeFirst_Assignment.Controllers
{
    public class BookReviewsController : Controller
    {
        private readonly BookReviewContext _context;

        public BookReviewsController(BookReviewContext context)
        {
            _context = context;
        }

        // GET: BookReviews
        public async Task<IActionResult> Index()
        {
            return View(await _context.BookReviews.ToListAsync());
        }

        // GET: BookReviews/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookReview = await _context.BookReviews
                .FirstOrDefaultAsync(m => m.ReviewId == id);
            if (bookReview == null)
            {
                return NotFound();
            }

            return View(bookReview);
        }

        // GET: BookReviews/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: BookReviews/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ReviewId,BookTitle,ReviewerName,Rating,Comments,ReviewDate")] BookReview bookReview)
        {
            if (ModelState.IsValid)
            {
                _context.Add(bookReview);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(bookReview);
        }

        // GET: BookReviews/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookReview = await _context.BookReviews.FindAsync(id);
            if (bookReview == null)
            {
                return NotFound();
            }
            return View(bookReview);
        }

        // POST: BookReviews/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ReviewId,BookTitle,ReviewerName,Rating,Comments,ReviewDate")] BookReview bookReview)
        {
            if (id != bookReview.ReviewId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bookReview);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookReviewExists(bookReview.ReviewId))
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
            return View(bookReview);
        }

        // GET: BookReviews/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookReview = await _context.BookReviews
                .FirstOrDefaultAsync(m => m.ReviewId == id);
            if (bookReview == null)
            {
                return NotFound();
            }

            return View(bookReview);
        }

        // POST: BookReviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bookReview = await _context.BookReviews.FindAsync(id);
            if (bookReview != null)
            {
                _context.BookReviews.Remove(bookReview);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookReviewExists(int id)
        {
            return _context.BookReviews.Any(e => e.ReviewId == id);
        }
    }
}
