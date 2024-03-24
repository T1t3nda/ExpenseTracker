using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ExpenseTracker.Data;
using ExpenseTracker.Models;
using Syncfusion.EJ2;
namespace ExpenseTracker.Controllers
{
    public class TransactionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TransactionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Transaction
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Transactions.Include(t => t.Category);

            List<object> DataRange = new List<object>();
            DataRange.Add(new { Text = "5 Rows", Value = "5" });
            DataRange.Add(new { Text = "10 Rows", Value = "10" });
            DataRange.Add(new { Text = "25 Rows", Value = "25" });
            ViewBag.Data = DataRange;

            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Transaction/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions
                .Include(t => t.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // GET: Transaction/AddOrEdit
        public IActionResult AddOrEdit(int id = 0)
        {
            PopulateCategories();
            if (id == 0)
                return View(new Transaction());
            else
                return View(_context.Transactions.Find(id));
        }

        // POST: Transaction/AddOrEdit
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit([Bind("Id,Amount,Date,Note,CategoryId")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                if (transaction.Id == 0)
                    _context.Add(transaction);
                else
                    _context.Update(transaction);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            PopulateCategories();
            return View(transaction);
        }

        // GET: Transaction/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions
                .Include(t => t.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // POST: Transaction/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction != null)
            {
                _context.Transactions.Remove(transaction);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TransactionExists(int id)
        {
            return _context.Transactions.Any(e => e.Id == id);
        }
        [NonAction]
        public void PopulateCategories()
        {
            // Retrieve categories from your data source
            var categories = _context.Categories.ToList();

            // Create a SelectList from the categories
            //var categoryList = new SelectList(categories, "Id", "TitleWithIcon");

            // Pass the SelectList to the View
            ViewBag.CategoryList = categories;

            //// Retrieve categories from your data source
            //var categories = _context.Categories.ToList();

            //// Create a list of strings to hold the TitleWithIcon values
            //List<string> categoryTitles = new List<string>();
            //foreach (var category in categories)
            //{
            //    categoryTitles.Add(category.TitleWithIcon);
            //}

            //// Pass the list to the View
            //ViewBag.CategoryTitles = categoryTitles;


        }
    }
}
