using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RentACarSystem.Data;
using RentACarSystem.Data.Entity;

namespace RentACarSystem.Controllers
{
    [Authorize]
    public class CarsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment webHostEnvironment;

        public CarsController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            webHostEnvironment = hostEnvironment;
        }

        // GET: Cars
        public async Task<IActionResult> Index(string Brand, string Model, int startYear, int endYear, int seats, double startPrice, double endPrice)
        {
            
            ViewData["CurrentFilter1"] = Brand;
            ViewData["CurrentFilter2"] = Model;
            ViewData["CurrentFilter3"] = startYear;
            ViewData["CurrentFilter4"] = endYear;
            ViewData["CurrentFilter5"] = seats;
            ViewData["CurrentFilter6"] = startPrice;
            ViewData["CurrentFilter7"] = endPrice;
            var container = new List<Car>();
            if (Brand != null)
            {
                container = await _context.Cars.Where(c => c.Brand == Brand).ToListAsync();
            }
            if (Model != null)
            {
                if (container.Count > 0)
                {
                    container = container.Where(c => c.Model == Model).ToList();
                }
                else
                {
                    container = await _context.Cars.Where(c => c.Model == Model).ToListAsync();
                }
            }
            if (startYear != default(int) && endYear != default(int))
            {
                if (container.Count > 0)
                {
                    container = container.Where(c => c.YearOfProduction>=startYear && c.YearOfProduction<=endYear).ToList();
                }
                else
                {
                    container = await _context.Cars.Where(c => c.YearOfProduction >= startYear && c.YearOfProduction <= endYear).ToListAsync();
                }
            }
            if (seats != default(int))
            {
                if (container.Count > 0)
                {
                    container = container.Where(c => c.Seats == seats).ToList();
                }
                else
                {
                    container = await _context.Cars.Where(c => c.Seats == seats).ToListAsync();
                }
            }
            if (startPrice != default(double) && endPrice != default(double))
            {
                if (container.Count > 0)
                {
                    container = container.Where(c => c.PricePerDay >=startPrice && c.PricePerDay <=endPrice).ToList();
                }
                else
                {
                    container = await _context.Cars.Where(c => c.PricePerDay >= startPrice && c.PricePerDay <= endPrice).ToListAsync();
                }
            }

            if (container.Count > 0)
            {
                return View(container);
            }
            return View(await _context.Cars.ToListAsync());
        }

        // GET: Cars/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _context.Cars
                .FirstOrDefaultAsync(m => m.Id == id);
            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }


        // GET: Cars/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Cars/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> Create([Bind("Brand,Model,YearOfProduction,Seats,Description,PricePerDay,Image,Id")] Car car, List<IFormFile> files)
        {
            if (ModelState.IsValid)
            {
                string filePath = "";
                if (files.Count > 0)
                {
                    var formFile = files[0];
                    if (formFile.Length > 0)
                    {
                        // this line is needed for the proper creation of the file int wwwroot/images
                        string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images");
                        filePath = Path.Combine(uploadsFolder, formFile.FileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await formFile.CopyToAsync(stream);
                        }
                    }
                    car.Image = formFile.FileName;
                }
                _context.Add(car);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(car);
        }

        // GET: Cars/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _context.Cars.FindAsync(id);
            if (car == null)
            {
                return NotFound();
            }
            return View(car);
        }

        // POST: Cars/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Brand,Model,YearOfProduction,Seats,Description,PricePerDay,Image,Id")] Car car, List<IFormFile> files)
        {
            if (id != car.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    string filePath = "";
                    if (files.Count > 0)
                    {
                        var formFile = files[0];
                        if (formFile.Length > 0)
                        {
                            // this line is needed for the proper creation of the file int wwwroot/images
                            string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images");
                            filePath = Path.Combine(uploadsFolder, formFile.FileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await formFile.CopyToAsync(stream);
                            }
                        }
                        car.Image = formFile.FileName;
                    }
                    _context.Update(car);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarExists(car.Id))
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
            return View(car);
        }

        // GET: Cars/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _context.Cars
                .FirstOrDefaultAsync(m => m.Id == id);
            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }

        // POST: Cars/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            _context.Cars.Remove(car);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CarExists(int id)
        {
            return _context.Cars.Any(e => e.Id == id);
        }
    }
}
