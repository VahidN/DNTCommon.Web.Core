using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace DNTCommon.Web.Core;

/// <summary>
///     Anti Dos Firewall Extensions
/// </summary>
public static class AntiDosFirewallExtensions
{
    private const string AllowedCharactersPattern = @"^[a-zA-Z0-9\s\-\._\(\),\/:\+;'""\[\]=]*$";

    // کامپایل کردن Regex برای عملکرد بهتر
    private static readonly Regex AllowedCharsRegex = new(
        AllowedCharactersPattern, RegexOptions.Compiled, TimeSpan.FromSeconds(7));

    /// <summary>
    ///     Known Bot Keywords
    /// </summary>
    public static readonly string[] KnownBotKeywords =
    [
        // 1. عبارات کلیدی عمومی (General Keywords)
        "bot", "crawler", "spider", "robot", "fetcher", "preview", "scrape", "search", "analyzer", "validator",
        "monitor", "archiver", "proxy", "agent",

        // 2. خزنده‌های موتورهای جستجو و ابزارهای سئو (Search Engines & SEO Tools)
        "googlebot", "bingbot", "slurp", // Yahoo! Slurp
        "yandex", // Yandex Bot
        "baiduspider", // Baidu Bot
        "duckduckbot", "msnbot", "exabot", "dotbot", // DotBot
        "mj12bot", // Majestic SEO
        "semrush", // SEMrush Bot
        "ahrefs", // Ahrefs Bot
        "screaming frog", // SEO Tool
        "petalbot", // PetalBot (از Huawei)
        "ia_archiver", // Internet Archive
        "mediapartners-google", // Google AdSense
        "seznam", // Seznam Bot

        // 3. ربات‌های شبکه‌های اجتماعی و پلتفرم‌ها (Social Media & Platforms)
        "facebookexternalhit", "facebot", "twitterbot", "linkedinbot", "pinterestbot",
        "whatsapp", // برای تولید پیش‌نمایش لینک
        "slackbot", "applebot", // Siri and Spotlight Search
        "amazonbot", // Amazon Crawler

        // 4. ربات‌های هوش مصنوعی و LLM (AI & LLM Crawlers)
        "gptbot", // OpenAI GPT Training
        "oai-searchbot", // OpenAI Search
        "chatgpt-user", // ChatGPT
        "anthropic-ai", // Anthropic (Claude)
        "claudebot", "perplexitybot",
        "google-extended", // Google AI/Gemini Training (robots.txt token, but good to include)

        // 5. کتابخانه‌ها و ابزارهای خط فرمان متداول (Common Libraries & CLI Tools)
        "curl", // Command-line tool
        "wget", // Command-line tool
        "python-requests", // Python HTTP library
        "goutte", // PHP library
        "lwp", // Perl library (libwww)
        "java", // Java/HttpClient
        "okhttp", // Android/Java
        "go-http-client", // Go Language
        "ruby" // Ruby HTTP client
    ];

    /// <summary>
    ///     Adds IAntiDosFirewall to IServiceCollection.
    /// </summary>
    public static IServiceCollection AddAntiDosFirewall(this IServiceCollection services)
    {
        services.AddSingleton<IAntiDosFirewall, AntiDosFirewall>();

        return services;
    }

    /// <summary>
    ///     Determines whether this request is coming from a Bot
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    public static bool IsBot(this BotScoreLevel level)
        => level is BotScoreLevel.LikelyBot or BotScoreLevel.Bot or BotScoreLevel.HighRiskBot;

    /// <summary>
    ///     Determines whether this request is coming from a Bot
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public static bool IsBot(this HttpContext? context) => context?.Request.GetBotScore().IsBot() == true;

    /// <summary>
    ///     Determines whether this request is coming from a Bot
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public static bool IsBot(this HttpRequest? request) => request?.GetBotScore().IsBot() == true;

    /// <summary>
    ///     Determines whether this request is coming from a Bot
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public static BotScoreLevel GetBotScore(this HttpRequest? request)
    {
        if (request is null)
        {
            return BotScoreLevel.LikelyBot;
        }

        var score = GetUserAgentAnalysisScore(request);
        score += GetBrowserHeadersScore(request);
        score += GetHeadersCountScore(request);
        score += GetRefererLogicScore(request);
        score += GetRequestTypeHeuristicsScore(request);
        score += GetPathScanDetectionScore(request);

        score = score > 100 ? 100 : score;

        return MapScoreToLevel(score);
    }

    private static int GetPathScanDetectionScore(HttpRequest request)
    {
        var score = 0;
        var path = request.Path.Value ?? "";

        if (path.Contains(value: "wp-", StringComparison.OrdinalIgnoreCase))
        {
            score += 20;
        }

        return score;
    }

    private static int GetRequestTypeHeuristicsScore(HttpRequest request)
    {
        var score = 0;

        if (string.Equals(request.Method, b: "POST", StringComparison.Ordinal) &&
            !request.Headers.ContainsKey(key: "Origin"))
        {
            score += 10;
        }

        if (string.Equals(request.Method, b: "GET", StringComparison.Ordinal) && request.Query.Count > 10)
        {
            score += 5;
        }

        return score;
    }

    private static int GetRefererLogicScore(HttpRequest request)
    {
        var score = 0;
        var referer = request.Headers.Referer.ToString();

        if (string.Equals(request.Method, b: "GET", StringComparison.Ordinal) && string.IsNullOrEmpty(referer))
        {
            var isHtmlPage = request.Path == "/" ||
                             request.Path.Value?.EndsWith(value: ".html", StringComparison.OrdinalIgnoreCase) == true ||
                             request.Path.Value?.EndsWith(value: ".htm", StringComparison.OrdinalIgnoreCase) == true;

            if (isHtmlPage)
            {
                score += 10;
            }
        }

        return score;
    }

    private static int GetHeadersCountScore(HttpRequest request)
    {
        var score = 0;

        switch (request.Headers.Count)
        {
            case < 5:
                score += 20;

                break;
            case < 8:
                score += 10;

                break;
        }

        return score;
    }

    private static int GetBrowserHeadersScore(HttpRequest request)
    {
        var score = 0;

        if (!request.Headers.ContainsKey(key: "Accept-Language"))
        {
            score += 10;
        }

        if (!request.Headers.ContainsKey(key: "Accept-Encoding"))
        {
            score += 10;
        }

        var hasSecFetch =
            request.Headers.Keys.Any(k => k.StartsWith(value: "Sec-Fetch-", StringComparison.OrdinalIgnoreCase));

        if (!hasSecFetch)
        {
            score += 15;
        }

        return score;
    }

    private static int GetUserAgentAnalysisScore(HttpRequest request)
    {
        var score = 0;
        var ua = request.Headers.UserAgent.ToString().ToLower(CultureInfo.InvariantCulture);

        if (string.IsNullOrEmpty(ua))
        {
            score += 25;
        }

        if (KnownBotKeywords.Any(b => ua.Contains(b, StringComparison.OrdinalIgnoreCase)))
        {
            score += 40;
        }

        if (ua.Length < 20)
        {
            score += 10;
        }

        if (!AllowedCharsRegex.IsMatch(ua))
        {
            score += 20;
        }

        return score;
    }

    private static BotScoreLevel MapScoreToLevel(int score)
        => score switch
        {
            <= 20 => BotScoreLevel.Human,
            <= 40 => BotScoreLevel.Suspicious,
            <= 60 => BotScoreLevel.LikelyBot,
            <= 80 => BotScoreLevel.Bot,
            _ => BotScoreLevel.HighRiskBot
        };
}
