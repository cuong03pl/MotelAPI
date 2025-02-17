using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;

namespace Motel.Helpers
{
    public class GenerateSlug
    {
        public string CreateSlug(string slug)
        {
            var title = TextToSlug(slug);
            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            string uniqueSlug = $"{title}-{timestamp}";
            return uniqueSlug;
        }

        public string TextToSlug(string title)
        {
            title = RemoveDiacritics(title);
            title = title.ToLower();
            title = Regex.Replace(title, @"\s+", "-");
            title = Regex.Replace(title, @"[^\w\-]", "");
            title = Regex.Replace(title, @"-+", "-");
            return title;
        }
        private string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }
            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
