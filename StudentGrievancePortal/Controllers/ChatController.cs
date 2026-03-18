using Microsoft.AspNetCore.Mvc;
using StudentGrievancePortal.services;
using StudentGrievancePortal.Data;
using Microsoft.EntityFrameworkCore;

namespace StudentGrievancePortal.Controllers
{
    [ApiController]
    [Route("Chat")]
    public class ChatController : Controller
    {
        private readonly GeminiService _gemini;
        private readonly ApplicationDbContext _db;

        public ChatController(GeminiService gemini, ApplicationDbContext db)
        {
            _gemini = gemini;
            _db = db;
        }

        [HttpPost("Ask")]
        public async Task<IActionResult> Ask([FromBody] ChatRequest req)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.message))
            {
                return BadRequest(new { reply = "Please enter a message." });
            }

            if (req.message.Length > 300)
            {
                return BadRequest(new { reply = "Message too long. Please keep it under 300 characters." });
            }

            try
            {
                StudentStats? stats = null;
                if (req.userId.HasValue)
                {
                    var list = await _db.Grievances
                        .Where(g => g.StudentId == req.userId.Value)
                        .OrderByDescending(g => g.CreatedAt)
                        .ToListAsync();

                    var total = list.Count;
                    var resolved = list.Count(g => string.Equals(g.Status, "Resolved", StringComparison.OrdinalIgnoreCase));
                    var pending = total - resolved;

                    stats = new StudentStats { Total = total, Pending = pending, Resolved = resolved };

                    var lower = req.message.ToLower();

                    if (lower.Contains("list") && (lower.Contains("unresolved") || lower.Contains("pending") || lower.Contains("open")))
                    {
                        var unresolved = list.Where(g => !string.Equals(g.Status, "Resolved", StringComparison.OrdinalIgnoreCase)).ToList();

                        if (!unresolved.Any())
                        {
                            return Ok(new { reply = "You have no unresolved tickets. All your grievances are resolved or you haven't submitted any." });
                        }

                        var sb = new System.Text.StringBuilder();
                        sb.AppendLine($"You have {unresolved.Count} unresolved grievance{(unresolved.Count == 1 ? "" : "s")}:\n");

                        foreach (var g in unresolved.Take(10))
                        {
                            var ticket = !string.IsNullOrWhiteSpace(g.TicketNumber) ? g.TicketNumber : $"GRV-{g.GrievanceId}";
                            var created = g.CreatedAt.ToString("yyyy-MM-dd");
                            sb.AppendLine($"{ticket}: {g.Subject} ({g.Status}) - Submitted: {created}");
                        }

                        if (unresolved.Count > 10)
                        {
                            sb.AppendLine($"\nAnd {unresolved.Count - 10} more unresolved grievance(s). Visit 'My Grievances' to view all.");
                        }

                        return Ok(new { reply = sb.ToString() });
                    }

                    if (lower.Contains("how many") || lower.Contains("how many grievances") || lower.Contains("count of grievances") || lower.Contains("total grievances"))
                    {
                        var quick = $"You have submitted {total} grievance{(total == 1 ? "" : "s")}. {pending} pending, {resolved} resolved.";
                        return Ok(new { reply = quick });
                    }

                    if (lower.Contains("status of my") || lower.Contains("what is the status") || (lower.Contains("pending") && lower.Contains("grievance")))
                    {
                        var quick = $"You have {pending} pending grievance{(pending == 1 ? "" : "s")} and {resolved} resolved.";
                        return Ok(new { reply = quick });
                    }
                }

                var reply = await _gemini.AskGemini(req.message, stats);

                return Ok(new { reply });
            }
            catch
            {
                return StatusCode(500, new
                {
                    reply = "⚠️ AI service is temporarily unavailable."
                });
            }
        }
    }

    public class ChatRequest
    {
        public string message { get; set; }
        public int? userId { get; set; }
    }
}