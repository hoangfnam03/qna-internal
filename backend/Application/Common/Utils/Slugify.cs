using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Application.Common.Utils
{
    public static class Slugify
    {
        private static readonly Regex NonAlphaNum = new(@"[^a-z0-9\s-]", RegexOptions.Compiled);
        private static readonly Regex MultiSpace = new(@"\s+", RegexOptions.Compiled);
        private static readonly Regex MultiDash = new(@"-+", RegexOptions.Compiled);

        public static string From(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;

            // remove diacritics
            var normalized = input.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (var c in normalized)
            {
                var uc = CharUnicodeInfo.GetUnicodeCategory(c);
                if (uc != UnicodeCategory.NonSpacingMark) sb.Append(c);
            }

            var s = sb.ToString().Normalize(NormalizationForm.FormC);

            s = s.Replace('đ', 'd').Replace('Đ', 'd');

            s = NonAlphaNum.Replace(s, "");
            s = MultiSpace.Replace(s, " ").Trim();
            s = s.Replace(' ', '-');
            s = MultiDash.Replace(s, "-").Trim('-');

            return s;
        }
    }
}
