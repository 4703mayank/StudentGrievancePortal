using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace StudentGrievancePortal.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var roleId = HttpContext.Session.GetInt32("UserRole");

            // Redirect logged-in users to their respective dashboards
            if (roleId == 1) return RedirectToAction("Index", "Student");
            if (roleId == 2) return RedirectToAction("Dashboard", "Coordinator");

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}