using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace StudentGrievancePortal.Services
{
    public static class PrioritySuggester
    {
        public static (string Priority, double Confidence) SuggestPriorityWithConfidence(string title, string description, string? category = null)
        {
            var text = (title + " " + description + " " + (category ?? "")).ToLowerInvariant();

            var High = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
            {
                { "safety", 6 }, { "injury", 6 }, { "emergency", 7 }, { "harassment", 6 },
                { "threat", 7 }, { "bomb", 9 }, { "violence", 7 }, { "assault", 7 },
                { "sexual harassment", 8 }, { "rape", 9 }, { "abuse", 7 }
            };

            var Medium = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
            {
                { "id card", 3 }, { "idcard", 3 }, { "cheating", 5 }, { "fraud", 5 },
                { "fire", 5 }, { "collapse", 6 }, { "powercut", 4 }, { "power cut", 4 },
                { "power outage", 4 }, { "leak", 4 }, { "hospital", 6 }, { "exam malpractice", 5 }
            };

            var Low = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
            {
                { "delay", 2 }, { "facility", 2 }, { "access", 2 }, { "schedule", 2 },
                { "billing", 2 }, { "grading", 2 }, { "internet", 2 }, { "wifi", 2 },
                { "hostel", 2 }, { "accommodation", 2 }, { "mess", 2 }
            };

            double scoreFor(Dictionary<string, double> dict)
            {
                double score = 0;
                foreach (var kv in dict)
                {
                    var pattern = @"\b" + Regex.Escape(kv.Key.ToLowerInvariant()) + @"\b";
                    var matches = Regex.Matches(text, pattern, RegexOptions.IgnoreCase);

                    if (matches.Count > 0)
                    {
                        score += kv.Value * matches.Count;
                    }
                }
                return score;
            }

            var scores = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
            {
                { "High", scoreFor(High) },
                { "Medium", scoreFor(Medium) },
                { "Low", scoreFor(Low) }
            };

            if (scores.Values.All(s => s <= 0))
                return ("Low", 1.0);

            var ordered = scores.OrderByDescending(kv => kv.Value).ToList();
            var top = ordered[0];
            double topScore = top.Value;
            double total = scores.Sum(kv => kv.Value);

            double confidence = topScore <= 0 ? 0.0 : Math.Min(1.0, topScore / (total + 1e-6));

            return (top.Key, Math.Round(confidence, 3));
        }

        public static string SuggestPriority(string title, string description, string? category = null)
        {
            return SuggestPriorityWithConfidence(title, description, category).Priority;
        }
    }
}