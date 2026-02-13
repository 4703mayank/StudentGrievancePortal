using Microsoft.AspNetCore.Mvc;
using StudentGrievancePortal.Models;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace StudentGrievancePortal.Controllers
{
    public class AccountController : Controller
    {
        private readonly GrievanceContext _context;

        public AccountController(GrievanceContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            if (HttpContext.Session.GetInt32("UserId") != null)
            {
                int? roleId = HttpContext.Session.GetInt32("UserRole");
                if (roleId == 1) return RedirectToAction("Index", "Student");
                if (roleId == 2) return RedirectToAction("Dashboard", "Coordinator");
            }
            return View();
        }

        [HttpPost]
        public IActionResult Login(string erpId, string password, int userRole)
        {
            var user = _context.Users.FirstOrDefault(u =>
                u.ERP_Id == erpId &&
                u.PasswordHash == password &&
                u.RoleId == userRole);

            if (user != null)
            {
                HttpContext.Session.SetInt32("UserId", user.UserId);
                HttpContext.Session.SetString("UserName", user.FullName);
                HttpContext.Session.SetInt32("UserRole", user.RoleId);
                HttpContext.Session.SetInt32("UserDept", user.DeptId);

                if (user.RoleId == 1) 
                {
                    return RedirectToAction("Index", "Student");
                }
                else if (user.RoleId == 2) 
                {
                    return RedirectToAction("Dashboard", "Coordinator");
                }
            }

            ViewBag.Error = "Invalid Credentials or Role Selection. Please try again.";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }
    }
}