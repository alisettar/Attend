namespace Attend.Infrastructure.Extensions;

/// <summary>
/// String uzantı metodları - Türkçe karakter normalizasyonu için
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Türkçe karakterleri normalize eder ve küçük harfe çevirir.
    /// Arama işlemlerinde kullanılır.
    /// </summary>
    /// <param name="text">Normalize edilecek metin</param>
    /// <returns>Normalize edilmiş metin</returns>
    public static string NormalizeTurkish(this string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        return text
            .ToLowerInvariant()
            .Replace('ı', 'i')
            .Replace('İ', 'i')
            .Replace('ş', 's')
            .Replace('Ş', 's')
            .Replace('ğ', 'g')
            .Replace('Ğ', 'g')
            .Replace('ü', 'u')
            .Replace('Ü', 'u')
            .Replace('ö', 'o')
            .Replace('Ö', 'o')
            .Replace('ç', 'c')
            .Replace('Ç', 'c');
    }
}
