using Microsoft.AspNetCore.Mvc;
using StudentGrievancePortal.Models;
using System.Linq;

namespace StudentGrievancePortal.Controllers
{
    public class CoordinatorController : Controller
    {
        private readonly GrievanceContext _context;

        public CoordinatorController(GrievanceContext context)
        {
            _context = context;
        }

        public IActionResult Dashboard()
        {
            var deptId = HttpContext.Session.GetInt32("UserDept");
            if (deptId == null) return RedirectToAction("Login", "Account");

            // ANONYMITY LOGIC: We fetch grievances for this CC department
            // but we DO NOT include any Student information in the query.
            var pendingGrievances = _context.Grievances
                .Where(g => g.AssignedDeptId == deptId)
                .OrderByDescending(g => g.CreatedAt)
                .ToList();

            return View(pendingGrievances);
        }

        [HttpPost]
        public IActionResult Resolve(int id, string resolutionDetails)
        {
            var grievance = _context.Grievances.Find(id);
            if (grievance != null)
            {
                grievance.Status = "Resolved";
                grievance.ResolutionDetails = resolutionDetails;
                grievance.UpdatedAt = DateTime.Now;

                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Dashboard));
        }
    }
}