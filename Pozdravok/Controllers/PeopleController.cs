using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Pozdravok.Models;

namespace Pozdravok.Controllers
{
    public class PeopleController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PeopleController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Birthday(string filterType)
        {
            ViewData["BirthDateSortParam"] = filterType == "week" ? "month" : "halfYear";
            var currentDate = DateTime.Today;

            IQueryable<People> birthdaysQuery = _context.Birthdays;

            switch (filterType)
            {
                case "week":
                    birthdaysQuery = birthdaysQuery
                        .Where(birthday => birthday.BirthDate >= currentDate && birthday.BirthDate <= currentDate.AddDays(7));
                    break;
                case "month":
                    birthdaysQuery = birthdaysQuery
                        .Where(birthday => birthday.BirthDate.Month == currentDate.Month);
                    break;
                case "halfYear":
                    birthdaysQuery = birthdaysQuery
                        .Where(birthday => birthday.BirthDate >= currentDate.AddMonths(-6));
                    break;
                default:
                    // По умолчанию показываем за неделю
                    birthdaysQuery = birthdaysQuery
                        .Where(birthday => birthday.BirthDate >= currentDate && birthday.BirthDate <= currentDate.AddDays(7));
                    break;
            }
            var sortedBirthdays = birthdaysQuery.ToList();
           
            return View("Birthday", sortedBirthdays);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Sort(string sortOrder)
        {
            ViewData["NameSortParam"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["SurnameSortParam"] = sortOrder == "Surname" ? "surname_desc" : "Surname";
            ViewData["BirthDateSortParam"] = sortOrder == "BirthDate" ? "birthdate_desc" : "BirthDate";

            IQueryable<People> peopleQuery = _context.Birthdays;

            switch (sortOrder)
            {
                case "name":
                    peopleQuery = peopleQuery.OrderBy(p => p.Name);
                    break;
                case "name_desc":
                    peopleQuery = peopleQuery.OrderByDescending(p => p.Name);
                    break;
                case "Surname":
                    peopleQuery = peopleQuery.OrderBy(p => p.Surname);
                    break;
                case "surname_desc":
                    peopleQuery = peopleQuery.OrderByDescending(p => p.Surname);
                    break;
                case "BirthDate":
                    peopleQuery = peopleQuery.OrderBy(p => p.BirthDate);
                    break;
                case "birthdate_desc":
                    peopleQuery = peopleQuery.OrderByDescending(p => p.BirthDate);
                    break;
                default:
                    peopleQuery = peopleQuery.OrderBy(p => p.Name);
                    break;
            }

            var sortedPeople = await peopleQuery.ToListAsync();
            return View("Index", sortedPeople);
        }

        public async Task<IActionResult> TodayBirth()
        {
            // Получите список ближайших дней рождений (например, за следующие 7 дней)
            var currentDate = DateTime.Today;
            var nextWeek = currentDate.AddDays(7);

            var upcomingBirthdays = await _context.Birthdays
                .Where(b => b.BirthDate >= currentDate && b.BirthDate <= nextWeek)
                .OrderBy(b => b.BirthDate)
                .ToListAsync();

            return View(upcomingBirthdays);
        }
        // GET: People
        public async Task<IActionResult> Index()
        {
            return View(await _context.Birthdays.ToListAsync());
        }

        // GET: People/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var people = await _context.Birthdays
                .FirstOrDefaultAsync(m => m.Id == id);
            if (people == null)
            {
                return NotFound();
            }

            return View(people);
        }

        // GET: People/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: People/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task <IActionResult> Create(People people, IFormFile Photo)
        {
            
                if (Photo != null && Photo.Length > 0)
                   
                { 
                    using (var memoryStream = new MemoryStream())
                    {
                        Photo.CopyTo(memoryStream);
                        people.Photo = memoryStream.ToArray();
                    }
                }
            _context.Birthdays.Add(people);
            _context.SaveChanges();

             return RedirectToAction("Index");
          
        }

        // GET: People/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var people = await _context.Birthdays.FindAsync(id);
            if (people == null)
            {
                return NotFound();
            }
            return View(people);
        }

        // POST: People/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,  People people, IFormFile photo)
        {
            if (id != people.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (photo != null && photo.Length > 0)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            photo.CopyTo(memoryStream);
                            people.Photo = memoryStream.ToArray();
                        }
                    }
                    _context.Update(people);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PeopleExists(people.Id))
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
            return View(people);
        }

        // GET: People/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var people = await _context.Birthdays
                .FirstOrDefaultAsync(m => m.Id == id);
            if (people == null)
            {
                return NotFound();
            }

            return View(people);
        }

        // POST: People/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var people = await _context.Birthdays.FindAsync(id);
            if (people != null)
            {
                _context.Birthdays.Remove(people);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PeopleExists(int id)
        {
            return _context.Birthdays.Any(e => e.Id == id);
        }
    }
}
