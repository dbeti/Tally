using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tally.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Tally.Models;
using Microsoft.AspNetCore.Authorization;

namespace Tally.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        } 
        public async Task<IActionResult> Index()
        {
            List<Course> courses = await _context.Course.Include(c => c.Professor).Include(c => c.CourseUsers).ToListAsync();
            if (User.IsInRole("Professor"))
            {
                var professorId = _userManager.GetUserId(User);
                courses = courses.Where(c => c.Professor.Id == professorId).ToList();
            }
            return View(courses);
        }
    }
}
