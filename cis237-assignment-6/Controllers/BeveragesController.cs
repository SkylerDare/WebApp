//Skyler Dare
//CIS237
//12/16/21
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using cis237_assignment_6.Models;

namespace cis237_assignment_6.Controllers
{
    [Authorize]
    public class BeveragesController : Controller
    {
        private readonly BeverageContext _context;

        public BeveragesController(BeverageContext context)
        {
            _context = context;
        }

        // GET: Beverages
        // Takes in the beverages from the database, accepts form data from the user from the filter text boxes
        // it then applies the filters to the beverages database and returns all items that pass the tests
        public async Task<IActionResult> Index()
        {
            DbSet<Beverage> BeveragesToFilter = _context.Beverages;

            string filterName = "";
            string filterPack = "";
            string filterMin = "";
            string filterMax = "";

            decimal min = 0.00m;
            decimal max = 1000.00m;

            if (!String.IsNullOrWhiteSpace(HttpContext.Session.GetString("session_name")))
            {
                filterName = HttpContext.Session.GetString("session_name");
            }
            if (!String.IsNullOrWhiteSpace(HttpContext.Session.GetString("session_pack")))
            {
                filterPack = HttpContext.Session.GetString("session_pack");
            }
            if (!String.IsNullOrWhiteSpace(HttpContext.Session.GetString("session_min")))
            {
                filterMin = HttpContext.Session.GetString("session_min");
                min = Decimal.Parse(filterMin);
            }
            if (!String.IsNullOrWhiteSpace(HttpContext.Session.GetString("session_max")))
            {
                filterMax = HttpContext.Session.GetString("session_max");
                max = Decimal.Parse(filterMax);
            }

            IList<Beverage> finalFiltered = await BeveragesToFilter.Where(
                beverage => beverage.Price >= min &&
                beverage.Price <= max &&
                beverage.Name.Contains(filterName) &&
                beverage.Pack.Contains(filterPack)).ToListAsync();

            ViewData["filterName"] = filterName;
            ViewData["filterPack"] = filterPack;
            ViewData["filterMin"] = filterMin;
            ViewData["filterMax"] = filterMax;

            return View(finalFiltered);
        }

        // GET: Beverages/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var beverage = await _context.Beverages
                .FirstOrDefaultAsync(m => m.Id == id);
            if (beverage == null)
            {
                return NotFound();
            }

            return View(beverage);
        }

        // GET: Beverages/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Beverages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Pack,Price,Active")] Beverage beverage)
        {
            if (ModelState.IsValid)
            {
                _context.Add(beverage);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(beverage);
        }

        // GET: Beverages/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var beverage = await _context.Beverages.FindAsync(id);
            if (beverage == null)
            {
                return NotFound();
            }
            return View(beverage);
        }

        // POST: Beverages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Name,Pack,Price,Active")] Beverage beverage)
        {
            if (id != beverage.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(beverage);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BeverageExists(beverage.Id))
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
            return View(beverage);
        }

        // GET: Beverages/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var beverage = await _context.Beverages
                .FirstOrDefaultAsync(m => m.Id == id);
            if (beverage == null)
            {
                return NotFound();
            }

            return View(beverage);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Filter()
        {
            string name = HttpContext.Request.Form["name"];
            string pack = HttpContext.Request.Form["pack"];
            string min = HttpContext.Request.Form["min"];
            string max = HttpContext.Request.Form["max"];

            HttpContext.Session.SetString("session_name", name);
            HttpContext.Session.SetString("session_pack", pack);
            HttpContext.Session.SetString("session_min", min);
            HttpContext.Session.SetString("session_max", max);

            return RedirectToAction(nameof(Index));
        }

        // POST: Beverages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var beverage = await _context.Beverages.FindAsync(id);
            _context.Beverages.Remove(beverage);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BeverageExists(string id)
        {
            return _context.Beverages.Any(e => e.Id == id);
        }
    }
}
