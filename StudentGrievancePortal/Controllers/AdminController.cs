using Microsoft.AspNetCore.Mvc;
using StudentGrievancePortal.Models;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;
using System;

namespace StudentGrievancePortal.Controllers
{
    public class AdminController : Controller
    {
        private readonly GrievanceContext _context;

        public AdminController(GrievanceContext context)
        {
            _context = context;
        }

        public IActionResult Summary()
        {
            var roleId = HttpContext.Session.GetInt32("UserRole");
            var deptId = HttpContext.Session.GetInt32("UserDept");

            if (roleId == null || roleId != 2)
                return RedirectToAction("Login", "Account");

            var allGrievances = _context.Grievances
                .Where(g => g.AssignedDeptId == deptId)
                .OrderByDescending(g => g.CreatedAt)
                .ToList();

            ViewBag.Total = allGrievances.Count;
            ViewBag.Resolved = allGrievances.Count(g => g.Status == "Resolved");
            ViewBag.Pending = allGrievances.Count(g => g.Status != "Resolved");

            ViewBag.High = allGrievances.Count(g => g.Priority == "High");
            ViewBag.Medium = allGrievances.Count(g => g.Priority == "Medium");
            ViewBag.Low = allGrievances.Count(g => g.Priority == "Low");

            var resolvedGrievances = allGrievances.Where(g => g.Status == "Resolved").ToList();
            if (resolvedGrievances.Any())
            {
                var avgDays = resolvedGrievances.Average(g => (g.UpdatedAt - g.CreatedAt).TotalDays);
                ViewBag.AvgSLA = Math.Round(avgDays, 1);
            }
            else
            {
                ViewBag.AvgSLA = 0;
            }

            ViewBag.Recent = allGrievances;
            return View();
        }

        public IActionResult ExportToCSV()
        {
            var deptId = HttpContext.Session.GetInt32("UserDept");
            if (deptId == null) return RedirectToAction("Login", "Account");

            var data = _context.Grievances
                .Where(g => g.AssignedDeptId == deptId)
                .OrderByDescending(g => g.CreatedAt)
                .ToList();

            var builder = new StringBuilder();
            builder.AppendLine("Ticket Number,Subject,Status,Priority,Date Submitted,Resolution Details");

            foreach (var item in data)
            {
                string resolution = item.ResolutionDetails?.Replace("\"", "'").Replace("\r\n", " ").Replace("\n", " ") ?? "No resolution provided";

                string dateFormatted = item.CreatedAt.ToString("dd-MMM-yyyy");

                builder.AppendLine($"{item.TicketNumber},{item.Subject},{item.Status},{item.Priority},{dateFormatted},\"{resolution}\"");
            }

            string fileName = $"BVICAM_Grievance_Report_{DateTime.Now:yyyyMMdd}.csv";
            return File(Encoding.UTF8.GetBytes(builder.ToString()), "text/csv", fileName);
        }
    }
}