using Microsoft.AspNetCore.Mvc;
using StudentGrievancePortal.Models;
using System.Linq;
using System.Net.Mail;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace StudentGrievancePortal.Controllers
{
    public class CoordinatorController : Controller
    {
        private readonly GrievanceContext _context;
        private readonly IConfiguration _config;
        private readonly ILogger<CoordinatorController> _logger;

        public CoordinatorController(GrievanceContext context, IConfiguration config, ILogger<CoordinatorController> logger)
        {
            _context = context;
            _config = config;
            _logger = logger;
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
            var grievance = _context.Grievances
                .Include(g => g.Student)
                .FirstOrDefault(g => g.GrievanceId == id);

            if (grievance != null)
            {
                grievance.Status = "Resolved";
                grievance.ResolutionDetails = resolutionDetails;
                grievance.UpdatedAt = DateTime.Now;

                _context.SaveChanges();

                // send email notification to student (if email configured)
                try
                {
                    var studentEmail = grievance.Student?.Email;
                    if (!string.IsNullOrEmpty(studentEmail))
                    {
                        var smtpHost = _config["EmailSettings:SmtpServer"];
                        var smtpPortStr = _config["EmailSettings:Port"];
                        var smtpUser = _config["EmailSettings:Username"];
                        var smtpPass = _config["EmailSettings:Password"];
                        var fromAddress = _config["EmailSettings:SenderEmail"] ?? smtpUser;
                        var senderName = _config["EmailSettings:SenderName"] ?? fromAddress;

                        int smtpPort = 587;
                        if (!string.IsNullOrEmpty(smtpPortStr) && int.TryParse(smtpPortStr, out var p)) smtpPort = p;

                        var mail = new MailMessage();
                        mail.From = new MailAddress(fromAddress, senderName);
                        mail.To.Add(studentEmail);
                        mail.Subject = $"Grievance #{grievance.TicketNumber} Resolved";

                        var dashboardUrl = Url.Action("Index", "Student", null, Request.Scheme);
                        mail.Body = $"Hello {grievance.Student?.FullName},\n\nYour grievance #{grievance.TicketNumber} has been marked as resolved.\n\nResolution details:\n{resolutionDetails}\n\nYou can view your grievances here: {dashboardUrl}\n\nRegards,\n{senderName}";
                        mail.IsBodyHtml = false;

                        using (var client = new SmtpClient(smtpHost, smtpPort))
                        {
                            client.EnableSsl = true;
                            client.UseDefaultCredentials = false;
                            client.DeliveryMethod = SmtpDeliveryMethod.Network;
                            client.Timeout = 10000;
                            if (!string.IsNullOrEmpty(smtpUser))
                            {
                                client.Credentials = new System.Net.NetworkCredential(smtpUser, smtpPass);
                            }
                            client.Send(mail);
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    // log email failures but don't break coordinator flow
                    _logger?.LogError(ex, "Failed to send resolution email for grievance {GrievanceId}", grievance.GrievanceId);
                }

                // Optional: show a toast to the coordinator as well
                TempData["SuccessMessage"] = $"Ticket #{grievance.TicketNumber} marked resolved.";
            }
            return RedirectToAction(nameof(Dashboard));
        }
    }
}