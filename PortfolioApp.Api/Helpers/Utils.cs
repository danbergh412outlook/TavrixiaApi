using PortfolioApp.Api.Models;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;

namespace PortfolioApp.Api.Helpers
{
    public class Utils
    {
        public static async Task<string> GenerateUniqueSlug(string newName, Survey? survey, Func<string, int?, Task<HashSet<string>>> getExistingSlugs)
        {
            if (newName != survey?.Name)
            {
                var newSlug = Slugify(newName);
                var currentSlug = Slugify(survey?.Name);

                if (newSlug != currentSlug)
                {
                    string uniqueSlug = newSlug;
                    int counter = 1;

                    var existingSlugs = await getExistingSlugs(newSlug, survey?.Id);

                    while (existingSlugs.Contains(uniqueSlug))
                    {
                        uniqueSlug = $"{newSlug}-{counter}";
                        counter++;
                    }

                    return uniqueSlug;
                }
            }

            return survey.UrlName;
        }
        public static string Slugify(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            text = text.ToLowerInvariant();
            text = Regex.Replace(text, @"[^a-z0-9\s-]", "");         // Remove invalid chars
            text = Regex.Replace(text, @"\s+", "-").Trim('-');      // Replace spaces with hyphens
            text = Regex.Replace(text, @"-+", "-");                 // Remove multiple hyphens

            return text;
        }
    }
}
