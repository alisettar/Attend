namespace Attend.Web.Extensions;

public static class DateTimeExtensions
{
    private static readonly TimeZoneInfo TurkeyTimeZone = 
        TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time");

    /// <summary>
    /// UTC tarihini Türkiye saatine (UTC+3) çevirir
    /// </summary>
    public static DateTime ToTurkeyTime(this DateTime utcDateTime)
    {
        if (utcDateTime.Kind != DateTimeKind.Utc)
        {
            utcDateTime = DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc);
        }

        return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, TurkeyTimeZone);
    }

    /// <summary>
    /// UTC tarihini Türkiye saatine (UTC+3) çevirir ve formatlı string döndürür
    /// </summary>
    public static string ToTurkeyTimeString(this DateTime utcDateTime, string format = "dd MMM yyyy HH:mm")
    {
        return utcDateTime.ToTurkeyTime().ToString(format);
    }
}
