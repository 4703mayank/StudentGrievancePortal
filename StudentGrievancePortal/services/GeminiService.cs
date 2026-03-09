using System.Text;
using System.Text.Json;

namespace StudentGrievancePortal.services
{
    public class GeminiService
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _http;

        public GeminiService(IConfiguration config)
        {
            _config = config;
            _http = new HttpClient();
        }

        public async Task<string> AskGemini(string message)
        {
            var lower = message.ToLower();

            if (lower.Contains("login"))
                return "Use your BVICAM ERP credentials to login. If you forgot your password, contact the admin office.";

            if (lower.Contains("submit grievance"))
                return "Go to Dashboard → Submit Grievance → Fill the form and submit.";

            if (lower.Contains("track"))
                return "You can track your grievance in Dashboard → My Grievances.";

            var apiKey = _config["Gemini:ApiKey"];
            Console.WriteLine("API KEY: " + apiKey);
            var url = $"https://generativelanguage.googleapis.com/v1/models/gemini-2.0-flash:generateContent?key={apiKey}";

            var messagePrompt =
                "You are a chatbot for BVICAM Student Grievance Portal. " +
                "Help students with grievance submission, tracking, login issues and portal navigation. " +
                "Give short and helpful answers.\n\n" +
                "Student Question: " + message;

            var body = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = messagePrompt }
                        }
                    }
                }
            };

            var json = JsonSerializer.Serialize(body);

            try
            {
                var response = await _http.PostAsync(
                    url,
                    new StringContent(json, Encoding.UTF8, "application/json")
                );

                var result = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                {
                    return "The chatbot is currently busy. Please try again in a moment.";
                }

                if (!response.IsSuccessStatusCode)
                {
                    return "AI service is temporarily unavailable.";
                }

                using var doc = JsonDocument.Parse(result);

                if (doc.RootElement.TryGetProperty("candidates", out var candidates))
                {
                    return candidates[0]
                        .GetProperty("content")
                        .GetProperty("parts")[0]
                        .GetProperty("text")
                        .GetString();
                }

                return "Sorry, I couldn't understand that. Please try again.";
            }
            catch
            {
                return "AI service is currently unavailable.";
            }
        }
    }
}