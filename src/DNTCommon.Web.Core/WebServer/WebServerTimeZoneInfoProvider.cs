namespace DNTCommon.Web.Core;

/// <summary>
///     WebServer TimeZoneInfo Provider
/// </summary>
public static class WebServerTimeZoneInfoProvider
{
    /// <summary>
    ///     Provides info about the web server's timezone
    /// </summary>
    /// <returns></returns>
    public static WebServerTimeZone GetTimezoneDetails()
    {
        var timeZoneInfo = TimeZoneInfo.Local;

        return new WebServerTimeZone
        {
            DisplayName = timeZoneInfo.DisplayName,
            BaseUtcOffset = timeZoneInfo.BaseUtcOffset,
            Language = CultureInfo.CurrentUICulture.Name,
            IsDaylightSavingTime = timeZoneInfo.IsDaylightSavingTime(DateTime.UtcNow)
        };
    }
}
