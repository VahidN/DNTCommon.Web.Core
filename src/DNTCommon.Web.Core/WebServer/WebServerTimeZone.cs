namespace DNTCommon.Web.Core;

/// <summary>
///     Provides info about the web server's timezone
/// </summary>
public class WebServerTimeZone
{
    /// <summary>
    ///     Indicates whether a specified date and time falls in the range of daylight saving time for the time zone of the
    ///     current TimeZoneInfo object.
    /// </summary>
    public bool IsDaylightSavingTime { set; get; }

    /// <summary>
    ///     Gets the general display name that represents the time zone.
    /// </summary>
    public string DisplayName { set; get; } = default!;

    /// <summary>
    ///     Gets the time difference between the current time zone's standard time and Coordinated Universal Time (UTC).
    /// </summary>
    public TimeSpan BaseUtcOffset { set; get; }

    /// <summary>
    ///     Gets the culture name in the format languagecode2-country/ regioncode2.
    /// </summary>
    public string Language { set; get; } = default!;
}