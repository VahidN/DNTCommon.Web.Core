using System.Text.RegularExpressions;
using System.Xml;

namespace DNTCommon.Web.Core;

/// <summary>
///     Parses RFC2822/RFC822 formatted dates.
/// </summary>
public static partial class FeedsDateTimeExtensions
{
    private const int TimeZones = 35;

    private static readonly string[] DateFormats =
    [
        // RFC 822 / 1123
        "r", "R", "ddd, dd MMM yyyy HH:mm:ss GMT", "ddd, dd MMM yyyy HH:mm:ss UTC", "ddd, dd MMM yyyy HH:mm:ss zzz",

        // ISO / Atom
        "yyyy-MM-dd'T'HH:mm:ssZ", "yyyy-MM-dd'T'HH:mm:sszzz", "yyyy-MM-dd'T'HH:mm:ss.fffZ",
        "yyyy-MM-dd'T'HH:mm:ss.fffzzz",

        // رایج ولی غیر استاندارد
        "yyyy-MM-dd HH:mm:ss", "yyyy/MM/dd HH:mm:ss", "yyyy-MM-dd HH:mm", "yyyy/MM/dd HH:mm",

        // عجیب ولی دیده‌شده
        "ddd MMM dd HH:mm:ss GMT yyyy", "ddd MMM dd HH:mm:ss zzz yyyy", "ddd MMM dd HH:mm:ss Z yyyy"
    ];

    private static readonly Regex Rfc2822 = Rfc2822Regex();

    private static readonly List<string> Months =
    [
        "ZeroIndex", "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"
    ];

    private static readonly Tzb[] ZoneBias =
    [
        new(Zone: "GMT", Bias: 0), new(Zone: "UT", Bias: 0), new(Zone: "EST", -5 * 60), new(Zone: "EDT", -4 * 60),
        new(Zone: "CST", -6 * 60), new(Zone: "CDT", -5 * 60), new(Zone: "MST", -7 * 60), new(Zone: "MDT", -6 * 60),
        new(Zone: "PST", -8 * 60), new(Zone: "PDT", -7 * 60), new(Zone: "Z", Bias: 0), new(Zone: "A", -1 * 60),
        new(Zone: "B", -2 * 60), new(Zone: "C", -3 * 60), new(Zone: "D", -4 * 60), new(Zone: "E", -5 * 60),
        new(Zone: "F", -6 * 60), new(Zone: "G", -7 * 60), new(Zone: "H", -8 * 60), new(Zone: "I", -9 * 60),
        new(Zone: "K", -10 * 60), new(Zone: "L", -11 * 60), new(Zone: "M", -12 * 60), new(Zone: "N", 1 * 60),
        new(Zone: "O", 2 * 60), new(Zone: "P", 3 * 60), new(Zone: "Q", 4 * 60), new(Zone: "R", 3 * 60),
        new(Zone: "S", 6 * 60), new(Zone: "T", 3 * 60), new(Zone: "U", 8 * 60), new(Zone: "V", 3 * 60),
        new(Zone: "W", 10 * 60), new(Zone: "X", 3 * 60), new(Zone: "Y", 12 * 60)
    ];

    private static readonly char[] Separator = [':'];

    /// <summary>
    ///     Converts an ISO 8601 date to a DateTime object.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static DateTimeOffset? TryParseFeedDate(this string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return null;
        }

        if (DateTimeOffset.TryParse(value, CultureInfo.InvariantCulture,
                DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                out var result))
        {
            return result;
        }

        if (DateTimeOffset.TryParseExact(value, DateFormats, CultureInfo.InvariantCulture,
                DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                out result))
        {
            return result;
        }

        try
        {
            return value.ToRfc2822DateTime();
        }
        catch
        {
            try
            {
                return ToIso8601DateTime(value);
            }
            catch
            {
                return null;
            }
        }
    }

    /// <summary>
    ///     Converts an ISO 8601 date to a DateTime object. Helper method needed to
    ///     deal with timezone offset since they are unsupported by the
    ///     .NET Framework.
    /// </summary>
    /// <param name="dateTime">DateTime string</param>
    /// <returns>DateTime instance</returns>
    /// <exception cref="FormatException">On format errors parsing the datetime</exception>
    /// <remarks>
    ///     See also W3C note at: http://www.w3.org/TR/NOTE-datetime
    /// </remarks>
    public static DateTime? ToIso8601DateTime([NotNullIfNotNull(nameof(dateTime))] string? dateTime)
    {
        if (string.IsNullOrEmpty(dateTime))
        {
            return null;
        }

        //strip trailing 'Z' since we assume UTC
        dateTime = dateTime.EndsWith(value: 'Z') ? dateTime[..^1] : dateTime;

        var timeIndex = dateTime.IndexOf(value: ':', StringComparison.OrdinalIgnoreCase);

        if (timeIndex != -1)
        {
            var tzoneIndex = dateTime.IndexOf(value: '-', timeIndex);

            if (tzoneIndex == -1)
            {
                tzoneIndex = dateTime.IndexOf(value: '+', timeIndex);

                if (tzoneIndex != -1)
                {
                    //timezone is ahead of UTC

                    return AddOffset(offsetOp: "+", dateTime, tzoneIndex);
                }
            }
            else
            {
                //timezone is behind UTC

                return AddOffset(offsetOp: "-", dateTime, tzoneIndex);
            }
        }

        if (timeIndex == dateTime.LastIndexOf(value: ':'))
        {
            //check if seconds part is missing
            dateTime += ":00";
        }

        return XmlConvert.ToDateTime(dateTime, XmlDateTimeSerializationMode.Utc);
    }

    /// <summary>
    ///     Parse is able to parse RFC2822/RFC822 formatted dates.
    ///     It has a fallback mechanism: if the string does not match,
    ///     the normal DateTime.Parse() function is called.
    /// </summary>
    /// <param name="dateTimeString">DateTime String to parse</param>
    /// <returns>DateTime instance with date and time converted to Universal Time</returns>
    /// <exception cref="FormatException">On format errors parsing the datetime</exception>
    public static DateTime ToRfc2822DateTime(this string? dateTimeString)
    {
        if (string.IsNullOrEmpty(dateTimeString))
        {
            return DateTime.UtcNow.ToUniversalTime();
        }

        var m = Rfc2822.Match(dateTimeString);

        if (!m.Success)
        {
            return DateTime.Parse(dateTimeString, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
        }

        var day = int.Parse(m.Groups[groupnum: 1].Value, CultureInfo.InvariantCulture);
        var month = Months.IndexOf(m.Groups[groupnum: 2].Value);
        var year = int.Parse(m.Groups[groupnum: 3].Value, CultureInfo.InvariantCulture);

        // following year completion is compliant with RFC 2822.
        if (year < 50)
        {
            // following year completion is compliant with RFC 2822.
            year = 2000 + year;
        }
        else if (year < 1000)
        {
            // following year completion is compliant with RFC 2822.
            year = 1900 + year;
        }

        var hour = int.Parse(m.Groups[groupnum: 4].Value, CultureInfo.InvariantCulture);
        var minute = int.Parse(m.Groups[groupnum: 5].Value, CultureInfo.InvariantCulture);

        var second = int.Parse("0" + m.Groups[groupnum: 6].Value,
            CultureInfo.InvariantCulture); // optional (may get lenght zero)

        var zone = m.Groups[groupnum: 7].Value;

        var xd = new DateTime(year, month, day, hour, minute, second, DateTimeKind.Utc);

        return xd.AddHours(RfcTimeZoneToGmtBias(zone) * -1);
    }

    private static double RfcTimeZoneToGmtBias(string zone)
    {
        string s;

        if (zone.IndexOfAny(['+', '-']) == 0) // +hhmm format
        {
            var fact = zone[..1] == "-" ? -1 : 1;
            s = zone[1..].TrimEnd();

            double hh = Math.Min(val1: 23, int.Parse(s.AsSpan(start: 0, length: 2), CultureInfo.InvariantCulture));

            var mm = Math.Min(val1: 59, int.Parse(s.AsSpan(start: 2, length: 2), CultureInfo.InvariantCulture)) / 60D;

            return fact * (hh + mm);
        } // named format

        s = zone.ToUpper(CultureInfo.InvariantCulture).Trim();

        for (var i = 0; i < TimeZones; i++)
        {
            if (ZoneBias[i].Zone.Equals(s, StringComparison.OrdinalIgnoreCase))
            {
                return ZoneBias[i].Bias / 60D;
            }
        }

        return 0.0;
    }

    private static DateTime AddOffset(string offsetOp, string datetime, int tzoneIndex)
    {
        var offset = datetime[(tzoneIndex + 1)..].Split(Separator);

        datetime = datetime[..tzoneIndex];

        if (datetime.IndexOf(value: ':', StringComparison.OrdinalIgnoreCase) == datetime.LastIndexOf(value: ':'))
        {
            //check if seconds part is missing
            datetime += ":00";
        }

        var toReturn = XmlConvert.ToDateTime(datetime, XmlDateTimeSerializationMode.Unspecified);

        // just fix a common issue of feed publishers: they specify the timezone
        // as "-0300", not as defined by http://www.w3.org/TR/NOTE-datetime format
        // TZD  = time zone designator (Z or +hh:mm or -hh:mm)
        if (offset is [{ Length: 4 }])
        {
            var offset2 = new string[2];
            offset2[0] = offset[0][..2];
            offset2[1] = offset[0][2..];
            offset = offset2;
        }

        return offsetOp switch
        {
            "+" => toReturn.Subtract(new TimeSpan(int.Parse(offset[0], CultureInfo.InvariantCulture),
                int.Parse(offset[1], CultureInfo.InvariantCulture), seconds: 0)),
            "-" => toReturn.Add(new TimeSpan(int.Parse(offset[0], CultureInfo.InvariantCulture),
                int.Parse(offset[1], CultureInfo.InvariantCulture), seconds: 0)),
            _ => toReturn //.ToLocalTime(); //we treat all dates in feeds as if they are local time (later)
        };
    }

    /// <summary>
    ///     returns a date as integer in the format YYYYMMDD.
    /// </summary>
    /// <param name="date">The date.</param>
    /// <returns>int</returns>
    public static int DateAsInteger(DateTime date) => date.Year * 10000 + date.Month * 100 + date.Day;

    [GeneratedRegex(
        pattern:
        @"\s*(?:(?:Mon|Tue|Wed|Thu|Fri|Sat|Sun)\s*,\s*)?(\d{1,2})\s+(Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec)\s+(\d{2,})\s+(\d{2})\s*:\s*(\d{2})\s*(?::\s*(\d{2}))?\s+([+\-]\d{4}|UT|GMT|EST|EDT|CST|CDT|MST|MDT|PST|PDT|[A-IK-Z])",
        RegexOptions.Compiled, matchTimeoutMilliseconds: 3000)]
    private static partial Regex Rfc2822Regex();

    private sealed record Tzb(string Zone, int Bias);
}
