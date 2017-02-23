using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Tally.Data;
using Microsoft.AspNetCore.Identity;
using Tally.Models;
using Microsoft.AspNetCore.Authorization;

namespace Tally.Controllers
{
    public class CoursesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CoursesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager; 
        }

        // GET: Courses/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ViewData["Users"] = _context.CourseUser.Include(cu => cu.User).Include(cu => cu.Course).Where(cu => cu.CourseId == id).Select(cu => cu.User).ToList();
            var course = await _context.Course.Include(c => c.Professor).Include(c => c.Lectures).Include(c => c.CourseUsers).SingleOrDefaultAsync(m => m.CourseId == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // GET: Courses/Create
        [Authorize(Roles="Professor")]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        // POST: Courses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Professor")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CourseId,Description,Name,Professor")] Course course)
        {
            if (ModelState.IsValid)
            {
                course.Professor = await _userManager.GetUserAsync(User);
                _context.Add(course);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            return View(course);
        }

        [HttpPost]
        public async Task<IActionResult> Enroll(int id)
        {
            var course = _context.Course.Include(c => c.CourseUsers).FirstOrDefault(c => c.CourseId == id);
            var current = await _userManager.GetUserAsync(User);

            course.CourseUsers.Add(new CourseUser() { User = current, Course = course });
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

        // GET: Courses/Edit/5
        [Authorize(Roles = "Professor")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Course.SingleOrDefaultAsync(m => m.CourseId == id);
            if (course == null)
            {
                return NotFound();
            }
            return View(course);
        }

        // POST: Courses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Professor")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CourseId,Description,Name")] Course course)
        {
            if (id != course.CourseId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(course);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseExists(course.CourseId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", "Home");
            }
            return View(course);
        }

        // GET: Courses/Delete/5
        [Authorize(Roles = "Professor")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Course.SingleOrDefaultAsync(m => m.CourseId == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Professor")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var course = await _context.Course.SingleOrDefaultAsync(m => m.CourseId == id);
            _context.Course.Remove(course);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }

        private bool CourseExists(int id)
        {
            return _context.Course.Any(e => e.CourseId == id);
        }
    }
}
