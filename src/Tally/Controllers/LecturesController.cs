using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Tally.Data;
using Tally.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace Tally.Controllers
{
    public class LecturesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private UserManager<ApplicationUser> _userManager;

        public LecturesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Lectures
        [Authorize]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Lecture.ToListAsync());
        }

        // GET: Lectures/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var userId = _userManager.GetUserId(User);
            ViewData["UserID"] = userId;
            var lecture = await _context.Lecture.Include(l => l.Course).Include(l => l.Course.CourseUsers).SingleOrDefaultAsync(m => m.LectureId == id);
            var signature = await _context.Signature.Include(s => s.Lecture).Include(s => s.Student).SingleOrDefaultAsync(s => s.Lecture.LectureId == id && s.Student.Id == userId);
            ViewData["Location"] = signature != null ? signature.Location : null;
            ViewData["Users"] = _context.CourseUser.Include(cu => cu.User).Include(cu => cu.Course).Where(cu => cu.CourseId == lecture.Course.CourseId).Select(cu => cu.User).ToList();
            if (User.IsInRole("Professor"))
            {
                ViewData["Signatures"] = _context.Signature.Include(s => s.Student).Include(s => s.Lecture).Where(s => s.Lecture.LectureId == id).ToList();
            }

            if (lecture == null)
            {
                return NotFound();
            }

            return View(lecture);
        }

        // GET: Lectures/Create
        [Authorize(Roles = "Professor")]
        public IActionResult Create(int id)
        {
            var lecture = new Lecture();
            lecture.Course = _context.Course.FirstOrDefault(c => c.CourseId == id);
            return View(lecture);
        }

        // POST: Lectures/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LectureId,Description,StartDate,Title")] Lecture lecture, int id)
        {
            lecture.Course = _context.Course.FirstOrDefault(c => c.CourseId == id);
            ModelState.Remove("Course");
            if (ModelState.IsValid)
            {
                _context.Add(lecture);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Courses", new { id = id });
            }
            return View(lecture);
        }

        // GET: Lectures/Edit/5
        [Authorize(Roles = "Professor")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lecture = await _context.Lecture.Include(l => l.Course).SingleOrDefaultAsync(m => m.LectureId == id);
            ViewData["CourseId"] = lecture.Course.CourseId;
            if (lecture == null)
            {
                return NotFound();
            }
            return View(lecture);
        }

        // POST: Lectures/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Professor")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LectureId,Description,StartDate,Title")] Lecture lecture)
        {
            lecture.Course = _context.Course.FirstOrDefault(c => c.CourseId == id);
            ModelState.Remove("Course");
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lecture);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LectureExists(lecture.LectureId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", "Courses", new { id = id });
            }
            return View(lecture);
        }

        // GET: Lectures/Delete/5
        [Authorize(Roles = "Professor")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lecture = await _context.Lecture.Include(l => l.Course).SingleOrDefaultAsync(m => m.LectureId == id);
            if (lecture == null)
            {
                return NotFound();
            }

            return View(lecture);
        }

        // POST: Lectures/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Professor")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var lecture = await _context.Lecture.Include(l => l.Course).SingleOrDefaultAsync(m => m.LectureId == id);
            var courseid = lecture.Course.CourseId;
            _context.Lecture.Remove(lecture);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Courses", new { id = courseid });
        }

        private bool LectureExists(int id)
        {
            return _context.Lecture.Any(e => e.LectureId == id);
        }
    }
}
