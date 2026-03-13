using Microsoft.AspNetCore.Mvc;
using StudentGrievancePortal.services;

namespace StudentGrievancePortal.Controllers
{
    [ApiController]
    [Route("Chat")]
    public class ChatController : Controller
    {
        private readonly GeminiService _gemini;

        public ChatController(GeminiService gemini)
        {
            _gemini = gemini;
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
                var reply = await _gemini.AskGemini(req.message);

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
    }
}