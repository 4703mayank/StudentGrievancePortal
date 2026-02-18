using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using StudentGrievancePortal.Models;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System;
using System.Globalization;
using StudentGrievancePortal.Services;

namespace StudentGrievancePortal.Controllers
{
    public class StudentController : Controller
    {
        private readonly GrievanceContext _context;

        public StudentController(GrievanceContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var studentId = HttpContext.Session.GetInt32("UserId");
            if (studentId == null)
                return RedirectToAction("Login", "Account");

            var myGrievances = _context.Grievances
                .Where(g => g.StudentId == studentId)
                .OrderByDescending(g => g.CreatedAt)
                .ToList();

            var lastSeenUtc = DateTime.MinValue;
            var lastSeenStr = HttpContext.Session.GetString("StudentDashboardLastSeen");

            if (!string.IsNullOrEmpty(lastSeenStr) &&
                DateTime.TryParse(lastSeenStr, null, DateTimeStyles.RoundtripKind, out var parsed))
            {
                lastSeenUtc = parsed.ToUniversalTime();
            }

            var newlyResolved = myGrievances
                .Where(g => g.Status == "Resolved" &&
                            g.UpdatedAt.ToUniversalTime() > lastSeenUtc)
                .OrderByDescending(g => g.UpdatedAt)
                .ToList();

            if (newlyResolved.Any())
            {
                var recent = newlyResolved
                    .Where(g => (DateTime.UtcNow - g.UpdatedAt).TotalHours <= 12)
                    .ToList();

                if (recent.Any())
                {
                    if (recent.Count == 1)
                        TempData["SuccessMessage"] =
                            $"Your grievance #{recent.First().TicketNumber} has been resolved.";
                    else
                        TempData["SuccessMessage"] =
                            $"{recent.Count} grievances were resolved. Latest: #{recent.First().TicketNumber}.";
                }
                else
                {
                    TempData["SuccessMessage"] =
                        $"{newlyResolved.Count} grievances were resolved while you were away. Latest: #{newlyResolved.First().TicketNumber}.";
                }
            }

            HttpContext.Session.SetString(
                "StudentDashboardLastSeen",
                DateTime.UtcNow.ToString("o"));

            return View(myGrievances);
        }


        public IActionResult Create()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
                return RedirectToAction("Login", "Account");

            ViewBag.Departments = new SelectList(
                _context.Departments,
                "DeptId",
                "DeptName");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Grievance grievance)
        {
            var studentId = HttpContext.Session.GetInt32("UserId");
            if (studentId == null)
                return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                grievance.Priority = string.Empty;

                grievance.StudentId = (int)studentId;
                grievance.Status = "Submitted";
                grievance.CreatedAt = DateTime.Now;
                grievance.UpdatedAt = DateTime.Now;

                var deptName = _context.Departments
                    .FirstOrDefault(d => d.DeptId == grievance.AssignedDeptId)
                    ?.DeptName;

                var suggestion =
                    PrioritySuggester.SuggestPriorityWithConfidence(
                        grievance.Subject,
                        grievance.Description,
                        deptName);

                grievance.Priority = suggestion.Priority;

                _context.Grievances.Add(grievance);
                _context.SaveChanges();

                TempData["SuccessMessage"] =
                    $"Submission successful — Ticket #{grievance.TicketNumber}";

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Departments = new SelectList(
                _context.Departments,
                "DeptId",
                "DeptName");

            return View(grievance);
        }

        [HttpPost]
        public IActionResult SuggestPriority([FromBody] PriorityRequest request)
        {
            if (request == null)
                return BadRequest();

            var deptName = _context.Departments
                .FirstOrDefault(d => d.DeptId == request.AssignedDeptId)
                ?.DeptName;

            var result =
                PrioritySuggester.SuggestPriorityWithConfidence(
                    request.Subject,
                    request.Description,
                    deptName);

            return Json(new
            {
                priority = result.Priority,
                confidence = result.Confidence
            });
        }
    }
}