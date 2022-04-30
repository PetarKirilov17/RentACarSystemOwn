using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RentACarSystem.Data;
using RentACarSystem.Data.Entity;

namespace RentACarSystem.Controllers
{
    [Authorize]
    public class QueriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public QueriesController(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Queries
        public async Task<IActionResult> Index(Status status, bool isFilled)
        {
            
            var user = await _userManager.GetUserAsync(User);
            foreach (var q in _context.Queries) // TO CHECK
            {
                if (DateTime.Now < q.StartDate && q.StatusOfQuery != Status.Canceled )
                {
                    q.StatusOfQuery = Status.Waiting;
                }
                else if (DateTime.Now >= q.StartDate && DateTime.Now <= q.EndDate && q.StatusOfQuery != Status.Used)
                {
                    q.StatusOfQuery = Status.Active;
                }
                else if (DateTime.Now > q.EndDate && q.StatusOfQuery == Status.Active)
                {
                    q.StatusOfQuery = Status.Expired;
                }
                _context.Update(q);
            }
            
            await _context.SaveChangesAsync();
            if (isFilled)
            {
                ViewData["CurrentFilter1"] = status;
                if (User.IsInRole("Admin"))
                {
                    return View(await _context.Queries.Where(q => q.StatusOfQuery == status).ToListAsync());
                }
                else
                {
                    return View(await _context.Queries.Where(q => q.CreatorId == user.Id && q.StatusOfQuery == status).ToListAsync());
                }
            }
            else
            {
                if (User.IsInRole("Admin"))
                {
                    return View(await _context.Queries.ToListAsync());
                }
                else
                {
                    return View(await _context.Queries.Where(q => q.CreatorId == user.Id).ToListAsync());
                }
            }
           
        }

        // GET: Queries/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var query = await _context.Queries
                .FirstOrDefaultAsync(m => m.Id == id);
            if (query == null)
            {
                return NotFound();
            }

            return View(query);
        }

        // GET: Queries/Create
        public IActionResult Create(DateTime StartDate, DateTime EndDate)
        {
            ViewData["CurrentFilter"] = StartDate;
            ViewData["CurrentFilter2"] = EndDate;
            var freeCars = new List<Car>();
            bool isFilled = false;
            if (StartDate != default(DateTime) && EndDate != default(DateTime))
            {
                isFilled = true;
                foreach (var car in _context.Cars)
                {
                    bool isFree = true;
                    foreach (var q in _context.Queries.Where(q => q.CarId == car.Id))
                    {
                        if ((q.StartDate>=StartDate && q.StartDate <= EndDate) || (q.EndDate >= StartDate && q.EndDate <= EndDate))
                        {
                            isFree = false;
                            break;
                        }
                    }
                    if (isFree)
                    {
                        freeCars.Add(car);
                    }
                }
            }

            var free = freeCars
                .Select(s => new
                 {
                Text = s.Brand + " " + s.Model,
                Value = s.Id
                })
            .ToList();

            SelectList options = new SelectList(free, "Value", "Text");
            ViewBag.FreeCars = options;
            ViewBag.StartDate = StartDate;
            ViewBag.EndDate = EndDate;
            ViewBag.IsField = isFilled;
            return View();
        }

        public IActionResult ConfirmForCreation([Bind("StartDate,EndDate,PriceForThePeriod,StatusOfQuery,Id,CarId")] Query query)
        {
            int days = (int)Math.Ceiling((query.EndDate - query.StartDate).TotalDays);
            var car = _context.Cars.Find(query.CarId);
            double totalPrice = days * car.PricePerDay;
            ViewBag.TotalPrice = totalPrice;
            return View();
        }

        // POST: Queries/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StartDate,EndDate,PriceForThePeriod,StatusOfQuery,Id,CarId")] Query query)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                query.CreatorId = user.Id;
                _context.Add(query);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(query);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TakeCar(int id)
        {
            var query = _context.Queries.Find(id);
            if (id != query.Id)
            {
                return NotFound();
            }
            if (query.StatusOfQuery == Status.Waiting)
            {
                query.StatusOfQuery = Status.Active;
                _context.Update(query);
                await _context.SaveChangesAsync();

            }
            else
            {
                return BadRequest("The car cannot be taken with the current status of the query!");
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReturnCar(int id)
        {
            var query = _context.Queries.Find(id);
            if (id != query.Id)
            {
                return NotFound();
            }
            if (query.StatusOfQuery == Status.Active)
            {
                query.StatusOfQuery = Status.Used;
                _context.Update(query);
                await _context.SaveChangesAsync();
            }
            else
            {
                return BadRequest("The car cannot be return with the current status of the query!");
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelCar(int id)
        {
            var query = _context.Queries.Find(id);
            if (id != query.Id)
            {
                return NotFound();
            }
            if (query.StatusOfQuery == Status.Waiting)
            {
                query.StatusOfQuery = Status.Canceled;
                _context.Update(query);
                await _context.SaveChangesAsync();
            }
            else
            {
                return BadRequest("The car cannot be canceled if you expired the start date!");
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Queries/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var query = await _context.Queries.FindAsync(id);
            if (query == null)
            {
                return NotFound();
            }

            return View(query);
        }

        // POST: Queries/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("StartDate,EndDate,PriceForThePeriod,StatusOfQuery,Id,CarId,CreatorId")] Query query)
        {
            if (id != query.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(query);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QueryExists(query.Id))
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
            return View(query);
        }

        // GET: Queries/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var query = await _context.Queries
                .FirstOrDefaultAsync(m => m.Id == id);
            if (query == null)
            {
                return NotFound();
            }

            return View(query);
        }

        // POST: Queries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var query = await _context.Queries.FindAsync(id);
            _context.Queries.Remove(query);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool QueryExists(int id)
        {
            return _context.Queries.Any(e => e.Id == id);
        }
    }
}
