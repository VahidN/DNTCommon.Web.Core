namespace DNTCommon.Web.Core;

public static class UserAgentPool
{
    private static readonly string[] UAs =
    [
        // Chrome
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.6099.216 Safari/537.36",

        // Firefox
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:121.0) Gecko/20100101 Firefox/121.0",

        // Edge
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.6099.216 Safari/537.36 Edg/120.0.2210.133"
    ];

    public static string Random() => UAs[RandomNumberGenerator.GetInt32(UAs.Length)];
}
