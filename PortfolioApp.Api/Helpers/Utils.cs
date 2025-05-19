using PortfolioApp.Api.Models;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;

namespace PortfolioApp.Api.Helpers
{
    public class Utils
    {
        public static async Task<string> GenerateUniqueSlug(string newName, string oldName, string oldSlug, int? id, Func<string, int?, Task<HashSet<string>>> getExistingSlugs)
        {
            if (newName != oldName)
            {
                var newSlug = Slugify(newName);
                var currentSlug = Slugify(oldName);

                if (newSlug != currentSlug)
                {
                    string uniqueSlug = newSlug;
                    int counter = 1;

                    var existingSlugs = await getExistingSlugs(newSlug, id);

                    while (existingSlugs.Contains(uniqueSlug))
                    {
                        uniqueSlug = $"{newSlug}-{counter}";
                        counter++;
                    }

                    return uniqueSlug;
                }
            }

            return oldSlug;
        }
        public static string Slugify(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            text = text.ToLowerInvariant();

            // Step 1: Replace common word separators with space
            text = Regex.Replace(text, @"[\/_|+&:]", " ");

            // Step 2: Remove all remaining invalid characters (anything not a-z, 0-9, space or hyphen)
            text = Regex.Replace(text, @"[^a-z0-9 -]", "");

            // Step 3: Collapse multiple spaces or hyphens into a single hyphen
            text = Regex.Replace(text, @"[\s-]+", "-");

            // Step 4: Trim leading/trailing hyphens
            return text.Trim('-');
        }
    }
}
