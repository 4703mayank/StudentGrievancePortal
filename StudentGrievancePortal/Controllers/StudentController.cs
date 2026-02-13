using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using StudentGrievancePortal.Models;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace StudentGrievancePortal.Controllers
{
    public class StudentController : Controller
    {
        private readonly GrievanceContext _context;

        public StudentController(GrievanceContext context)
        {
            _context = context;
        }

        // Action to list student's own grievances
        public IActionResult Index()
        {
            var studentId = HttpContext.Session.GetInt32("UserId");
            if (studentId == null) return RedirectToAction("Login", "Account");

            var myGrievances = _context.Grievances
                .Where(g => g.StudentId == studentId)
                .OrderByDescending(g => g.CreatedAt)
                .ToList();

            return View(myGrievances);
        }

        // Action to show the submission form
        public IActionResult Create()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
                return RedirectToAction("Login", "Account");

            // Get departments so student can choose where to send the grievance
            ViewBag.Departments = new SelectList(_context.Departments, "DeptId", "DeptName");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Grievance grievance)
        {
            var studentId = HttpContext.Session.GetInt32("UserId");
            if (studentId == null) return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                grievance.StudentId = (int)studentId;
                grievance.Status = "Submitted";
                grievance.CreatedAt = DateTime.Now;
                grievance.UpdatedAt = DateTime.Now;

                _context.Grievances.Add(grievance);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Departments = new SelectList(_context.Departments, "DeptId", "DeptName");
            return View(grievance);
        }
    }
}