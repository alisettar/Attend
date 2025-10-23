using System.Globalization;
using System.Text.RegularExpressions;

namespace Attend.Domain.Extensions;

public static class StringExtensions
{
    private static readonly CultureInfo TurkishCulture = new("tr-TR");
    
    public static string NormalizeFullName(this string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        // Trim and remove multiple spaces
        var normalized = Regex.Replace(input.Trim(), @"\s+", " ");

        // Title case with Turkish culture (handles i/İ correctly)
        var textInfo = TurkishCulture.TextInfo;
        return textInfo.ToTitleCase(normalized.ToLower(TurkishCulture));
    }
}
