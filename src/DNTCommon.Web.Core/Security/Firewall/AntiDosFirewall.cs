using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetTools;

namespace DNTCommon.Web.Core;

/// <summary>
///     Anti Dos Firewall
/// </summary>
public class AntiDosFirewall : IAntiDosFirewall
{
    private readonly ICacheService _cacheService;
    private readonly ILogger<AntiDosFirewall> _logger;
    private AntiDosConfig _antiDosConfig;

    /// <summary>
    ///     Anti Dos Firewall
    /// </summary>
    public AntiDosFirewall(IOptionsMonitor<AntiDosConfig> antiDosConfig,
        ICacheService cacheService,
        ILogger<AntiDosFirewall> logger)
    {
        ArgumentNullException.ThrowIfNull(antiDosConfig);

        _antiDosConfig = antiDosConfig.CurrentValue;

        if (_antiDosConfig == null)
        {
            throw new ArgumentNullException(nameof(antiDosConfig),
                message: "Please add AntiDosConfig to your appsettings.json file.");
        }

        antiDosConfig.OnChange(options => { _antiDosConfig = options; });

        _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    ///     Such as `google` or `bing`.
    /// </summary>
    public bool IsGoodBot(AntiDosFirewallRequestInfo requestInfo)
    {
        ArgumentNullException.ThrowIfNull(requestInfo);

        if (string.IsNullOrWhiteSpace(requestInfo.UserAgent))
        {
            return false;
        }

        if (_antiDosConfig.GoodBotsUserAgents?.Any() != true)
        {
            return true;
        }

        return _antiDosConfig.GoodBotsUserAgents.Any(goodBot
            => requestInfo.UserAgent.Contains(goodBot, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    ///     Such as `asafaweb`.
    /// </summary>
    public (bool ShouldBlockClient, ThrottleInfo? ThrottleInfo) IsBadBot(AntiDosFirewallRequestInfo requestInfo)
    {
        var shouldBanBotHeader = ShouldBanBotHeaders(requestInfo);

        if (shouldBanBotHeader.ShouldBlockClient)
        {
            return (true, shouldBanBotHeader.ThrottleInfo);
        }

        var shouldBanUserAgent = ShouldBanUserAgent(requestInfo);

        if (shouldBanUserAgent.ShouldBlockClient)
        {
            return (true, shouldBanUserAgent.ThrottleInfo);
        }

        return (false, null);
    }

    /// <summary>
    ///     Is from remote localhost
    /// </summary>
    public bool? IsFromRemoteLocalhost(AntiDosFirewallRequestInfo requestInfo)
    {
        ArgumentNullException.ThrowIfNull(requestInfo);

        if (requestInfo.UrlReferrer == null)
        {
            return false;
        }

        if (!requestInfo.UrlReferrer.Host.Contains(value: "localhost", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        return !requestInfo.IP?.IsLocalIp();
    }

    /// <summary>
    ///     Such as `HTTP_ACUNETIX_PRODUCT`.
    /// </summary>
    public (bool ShouldBlockClient, ThrottleInfo? ThrottleInfo) HasUrlAttackVectors(
        AntiDosFirewallRequestInfo requestInfo)
    {
        ArgumentNullException.ThrowIfNull(requestInfo);

        if (string.IsNullOrWhiteSpace(requestInfo.RawUrl))
        {
            return (false, null);
        }

        if (requestInfo.RawUrl.EndsWith(value: ".php", StringComparison.OrdinalIgnoreCase))
        {
            return (true, new ThrottleInfo
            {
                ExpiresAt = GetCacheExpiresAt(),
                RequestsCount = _antiDosConfig.AllowedRequests,
                BanReason = $"{requestInfo.RawUrl} ends with .php and this an ASP.NET Core site!"
            });
        }

        if (_antiDosConfig.UrlAttackVectors?.Any() != true)
        {
            return (false, null);
        }

        var vector = _antiDosConfig.UrlAttackVectors.FirstOrDefault(attackVector
            => requestInfo.RawUrl.Contains(attackVector, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(vector))
        {
            return (true, new ThrottleInfo
            {
                ExpiresAt = GetCacheExpiresAt(),
                RequestsCount = _antiDosConfig.AllowedRequests,
                BanReason = $"UrlAttackVector: {vector}."
            });
        }

        return (false, null);
    }

    /// <summary>
    ///     Such as `HTTP_ACUNETIX_PRODUCT`.
    /// </summary>
    public (bool ShouldBlockClient, ThrottleInfo? ThrottleInfo) ShouldBanBotHeaders(
        AntiDosFirewallRequestInfo requestInfo)
    {
        ArgumentNullException.ThrowIfNull(requestInfo);

        if (_antiDosConfig.BadBotsRequestHeaders?.Any() != true || requestInfo.RequestHeaders == null)
        {
            return (false, null);
        }

        foreach (var headerkey in requestInfo.RequestHeaders.Keys)
        {
            var headerValue = requestInfo.RequestHeaders[headerkey];

            if (string.IsNullOrWhiteSpace(headerValue))
            {
                continue;
            }

            var botHeader = _antiDosConfig.BadBotsRequestHeaders.FirstOrDefault(badBotHeader
                => headerValue.ToString().Contains(badBotHeader, StringComparison.OrdinalIgnoreCase) ||
                   headerkey.Contains(badBotHeader, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(botHeader))
            {
                return (true, new ThrottleInfo
                {
                    ExpiresAt = GetCacheExpiresAt(),
                    RequestsCount = _antiDosConfig.AllowedRequests,
                    BanReason = $"BadBotRequestHeader: {botHeader}."
                });
            }
        }

        return (false, null);
    }

    /// <summary>
    ///     Such as `asafaweb`.
    /// </summary>
    public (bool ShouldBlockClient, ThrottleInfo? ThrottleInfo) ShouldBanUserAgent(
        AntiDosFirewallRequestInfo requestInfo)
    {
        ArgumentNullException.ThrowIfNull(requestInfo);

        if (string.IsNullOrWhiteSpace(requestInfo.UserAgent))
        {
            return (false, null); // for ping-backs validations
        }

        if (_antiDosConfig.BadBotsUserAgents?.Any() != true)
        {
            return (false, null);
        }

        var userAgent = _antiDosConfig.BadBotsUserAgents.FirstOrDefault(badBot
            => requestInfo.UserAgent.Contains(badBot, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(userAgent))
        {
            return (true, new ThrottleInfo
            {
                ExpiresAt = GetCacheExpiresAt(),
                RequestsCount = _antiDosConfig.AllowedRequests,
                BanReason = $"BadBotUserAgent: {userAgent}."
            });
        }

        var isLocal = IsFromRemoteLocalhost(requestInfo);

        if (isLocal == true)
        {
            return (true, new ThrottleInfo
            {
                ExpiresAt = GetCacheExpiresAt(),
                RequestsCount = _antiDosConfig.AllowedRequests,
                BanReason = "IsFromRemoteLocalhost."
            });
        }

        return (false, null);
    }

    /// <summary>
    ///     Should block client based on its info?
    /// </summary>
    public (bool ShouldBlockClient, ThrottleInfo? ThrottleInfo) ShouldBlockClient(
        AntiDosFirewallRequestInfo requestInfo)
    {
        ArgumentNullException.ThrowIfNull(requestInfo);

        var (shouldBanIp, banIpThrottleInfo) = ShouldBanIp(requestInfo);

        if (shouldBanIp)
        {
            return (true, ThrottleInfo: banIpThrottleInfo);
        }

        if (_antiDosConfig.IgnoreLocalHost && requestInfo.IsLocal)
        {
            return (false, null);
        }

        var (hasUrlAttackVector, urlAttackVectorThrottleInfo) = HasUrlAttackVectors(requestInfo);

        if (hasUrlAttackVector)
        {
            return (true, ThrottleInfo: urlAttackVectorThrottleInfo);
        }

        if (IsGoodBot(requestInfo))
        {
            return (false, null);
        }

        var (shouldBlockClient, throttleInfo) = IsBadBot(requestInfo);

        if (shouldBlockClient)
        {
            return (true, ThrottleInfo: throttleInfo);
        }

        var (isDosAttack, isDosAttackThrottleInfo) = IsDosAttack(requestInfo);

        if (isDosAttack)
        {
            return (true, ThrottleInfo: isDosAttackThrottleInfo);
        }

        return (false, null);
    }

    /// <summary>
    ///     Should block client based on its IP?
    /// </summary>
    public (bool ShouldBlockClient, ThrottleInfo? ThrottleInfo) ShouldBanIp(AntiDosFirewallRequestInfo requestInfo)
    {
        ArgumentNullException.ThrowIfNull(requestInfo);

        if (_antiDosConfig.BannedIPAddressRanges?.Any() != true)
        {
            return (false, null);
        }

        if (requestInfo.IP == null)
        {
            return (false, null);
        }

        var iPAddress = IPAddress.Parse(requestInfo.IP);

        foreach (var range in _antiDosConfig.BannedIPAddressRanges)
        {
            var ipRange = IPAddressRange.Parse(range);

            if (ipRange.Contains(iPAddress))
            {
                return (true, new ThrottleInfo
                {
                    ExpiresAt = GetCacheExpiresAt(),
                    RequestsCount = _antiDosConfig.AllowedRequests,
                    BanReason = $"IP: {requestInfo.IP} is in the `{range}` range."
                });
            }
        }

        return (false, null);
    }

    /// <summary>
    ///     Is this a dos attack?
    /// </summary>
    public (bool ShouldBlockClient, ThrottleInfo? ThrottleInfo) IsDosAttack(AntiDosFirewallRequestInfo requestInfo)
    {
        var key = GetCacheKey(requestInfo);
        var expiresAt = GetCacheExpiresAt();

        if (!_cacheService.TryGetValue<ThrottleInfo>(key, out var clientThrottleInfo) || clientThrottleInfo is null)
        {
            clientThrottleInfo = new ThrottleInfo
            {
                RequestsCount = 1,
                ExpiresAt = expiresAt
            };

            _cacheService.Add(key, nameof(AntiDosFirewall), clientThrottleInfo, expiresAt);

            return (false, clientThrottleInfo);
        }

        if (clientThrottleInfo.RequestsCount > _antiDosConfig.AllowedRequests)
        {
            clientThrottleInfo.BanReason = "IsDosAttack";
            _cacheService.Add(key, nameof(AntiDosFirewall), clientThrottleInfo, expiresAt);

            return (true, clientThrottleInfo);
        }

        clientThrottleInfo.RequestsCount++;
        _cacheService.Add(key, nameof(AntiDosFirewall), clientThrottleInfo, expiresAt);

        return (false, clientThrottleInfo);
    }

    /// <summary>
    ///     Returns cache's expirations date
    /// </summary>
    public DateTimeOffset GetCacheExpiresAt() => DateTimeOffset.UtcNow.AddMinutes(_antiDosConfig.DurationMin);

    /// <summary>
    ///     Returns cache's key
    /// </summary>
    public string GetCacheKey(AntiDosFirewallRequestInfo requestInfo)
    {
        ArgumentNullException.ThrowIfNull(requestInfo);

        return $"__AntiDos__{requestInfo.IP}";
    }

    /// <summary>
    ///     Logs a warning
    /// </summary>
    public void LogIt(ThrottleInfo? throttleInfo, AntiDosFirewallRequestInfo? requestInfo)
    {
        if (requestInfo == null)
        {
            return;
        }

        if (throttleInfo == null)
        {
            return;
        }

        if (!_antiDosConfig.LogErrors)
        {
            return;
        }

        if (throttleInfo.IsLogged)
        {
            return;
        }

        _cacheService.GetOrAdd($"__Anti_Dos__{$"{requestInfo.IP}_{requestInfo.UserAgent}".GetSha1Hash()}",
            nameof(AntiDosFirewall), () =>
            {
                _logger.LogWarning(
                    message: "Banned IP: {RequestInfoIP}, UserAgent: {RequestInfoUserAgent}. {ThrottleInfo}",
                    requestInfo.IP, requestInfo.UserAgent, throttleInfo);

                throttleInfo.IsLogged = true;
                _cacheService.Add(GetCacheKey(requestInfo), nameof(AntiDosFirewall), throttleInfo, GetCacheExpiresAt());

                return $"{requestInfo.IP}_{requestInfo.UserAgent}";
            }, DateTimeOffset.UtcNow.AddDays(days: 1));
    }
}