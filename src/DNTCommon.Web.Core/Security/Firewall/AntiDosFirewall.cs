using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetTools;

namespace DNTCommon.Web.Core;

/// <summary>
///     Anti Dos Firewall
/// </summary>
public class AntiDosFirewall : IAntiDosFirewall
{
    private readonly IOptionsSnapshot<AntiDosConfig> _antiDosConfig;
    private readonly ICacheService _cacheService;
    private readonly ILogger<AntiDosFirewall> _logger;

    /// <summary>
    ///     Anti Dos Firewall
    /// </summary>
    public AntiDosFirewall(
        IOptionsSnapshot<AntiDosConfig> antiDosConfig,
        ICacheService cacheService,
        ILogger<AntiDosFirewall> logger)
    {
        _antiDosConfig = antiDosConfig ?? throw new ArgumentNullException(nameof(antiDosConfig));
        if (_antiDosConfig.Value == null)
        {
            throw new ArgumentNullException(nameof(antiDosConfig),
                                            "Please add AntiDosConfig to your appsettings.json file.");
        }

        _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    ///     Such as `google` or `bing`.
    /// </summary>
    public bool IsGoodBot(AntiDosFirewallRequestInfo requestInfo)
    {
        if (requestInfo == null)
        {
            throw new ArgumentNullException(nameof(requestInfo));
        }

        if (string.IsNullOrWhiteSpace(requestInfo.UserAgent))
        {
            return false;
        }

        if (_antiDosConfig.Value.GoodBotsUserAgents?.Any() != true)
        {
            return true;
        }

        return _antiDosConfig.Value.GoodBotsUserAgents
                             .Any(goodBot => requestInfo.UserAgent.Contains(goodBot,
                                                                            StringComparison.OrdinalIgnoreCase));
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
        if (requestInfo == null)
        {
            throw new ArgumentNullException(nameof(requestInfo));
        }

        if (requestInfo.UrlReferrer == null)
        {
            return false;
        }

        if (!requestInfo.UrlReferrer.Host.Contains("localhost", StringComparison.OrdinalIgnoreCase))
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
        if (requestInfo == null)
        {
            throw new ArgumentNullException(nameof(requestInfo));
        }

        if (string.IsNullOrWhiteSpace(requestInfo.RawUrl))
        {
            return (false, null);
        }

        if (requestInfo.RawUrl.EndsWith(".php", StringComparison.OrdinalIgnoreCase))
        {
            return (true, new ThrottleInfo
                          {
                              ExpiresAt = GetCacheExpiresAt(),
                              RequestsCount = _antiDosConfig.Value.AllowedRequests,
                              BanReason = $"{requestInfo.RawUrl} ends with .php and this an ASP.NET Core site!",
                          });
        }

        if (_antiDosConfig.Value.UrlAttackVectors?.Any() != true)
        {
            return (false, null);
        }

        var vector = _antiDosConfig.Value.UrlAttackVectors
                                   .FirstOrDefault(attackVector =>
                                                       requestInfo.RawUrl.Contains(attackVector,
                                                        StringComparison.OrdinalIgnoreCase));
        if (!string.IsNullOrWhiteSpace(vector))
        {
            return (true, new ThrottleInfo
                          {
                              ExpiresAt = GetCacheExpiresAt(),
                              RequestsCount = _antiDosConfig.Value.AllowedRequests,
                              BanReason = $"UrlAttackVector: {vector}.",
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
        if (requestInfo == null)
        {
            throw new ArgumentNullException(nameof(requestInfo));
        }

        if (_antiDosConfig.Value.BadBotsRequestHeaders?.Any() != true ||
            requestInfo.RequestHeaders == null)
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

            var botHeader = _antiDosConfig.Value.BadBotsRequestHeaders
                                          .FirstOrDefault(badBotHeader =>
                                                              headerValue.ToString()
                                                                         .Contains(badBotHeader,
                                                                          StringComparison.OrdinalIgnoreCase) ||
                                                              headerkey.Contains(badBotHeader,
                                                               StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrWhiteSpace(botHeader))
            {
                return (true, new ThrottleInfo
                              {
                                  ExpiresAt = GetCacheExpiresAt(),
                                  RequestsCount = _antiDosConfig.Value.AllowedRequests,
                                  BanReason = $"BadBotRequestHeader: {botHeader}.",
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
        if (requestInfo == null)
        {
            throw new ArgumentNullException(nameof(requestInfo));
        }

        if (string.IsNullOrWhiteSpace(requestInfo.UserAgent))
        {
            return (false, null); // for ping-backs validations
        }

        if (_antiDosConfig.Value.BadBotsUserAgents?.Any() != true)
        {
            return (false, null);
        }

        var userAgent = _antiDosConfig.Value.BadBotsUserAgents
                                      .FirstOrDefault(badBot =>
                                                          requestInfo.UserAgent.Contains(badBot,
                                                           StringComparison.OrdinalIgnoreCase));
        if (!string.IsNullOrWhiteSpace(userAgent))
        {
            return (true, new ThrottleInfo
                          {
                              ExpiresAt = GetCacheExpiresAt(),
                              RequestsCount = _antiDosConfig.Value.AllowedRequests,
                              BanReason = $"BadBotUserAgent: {userAgent}.",
                          });
        }

        var isLocal = IsFromRemoteLocalhost(requestInfo);
        if (isLocal == true)
        {
            return (true, new ThrottleInfo
                          {
                              ExpiresAt = GetCacheExpiresAt(),
                              RequestsCount = _antiDosConfig.Value.AllowedRequests,
                              BanReason = "IsFromRemoteLocalhost.",
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
        if (requestInfo == null)
        {
            throw new ArgumentNullException(nameof(requestInfo));
        }

        var shouldBanIpResult = ShouldBanIp(requestInfo);
        if (shouldBanIpResult.ShouldBlockClient)
        {
            return (true, shouldBanIpResult.ThrottleInfo);
        }

        if (_antiDosConfig.Value.IgnoreLocalHost && requestInfo.IsLocal)
        {
            return (false, null);
        }

        var hasUrlAttackVectorsResult = HasUrlAttackVectors(requestInfo);
        if (hasUrlAttackVectorsResult.ShouldBlockClient)
        {
            return (true, hasUrlAttackVectorsResult.ThrottleInfo);
        }

        var isBadBotResult = IsBadBot(requestInfo);
        if (isBadBotResult.ShouldBlockClient)
        {
            return (true, isBadBotResult.ThrottleInfo);
        }

        if (IsGoodBot(requestInfo))
        {
            return (false, null);
        }

        var isDosAttackResult = IsDosAttack(requestInfo);
        if (isDosAttackResult.ShouldBlockClient)
        {
            return (true, isDosAttackResult.ThrottleInfo);
        }

        return (false, null);
    }

    /// <summary>
    ///     Should block client based on its IP?
    /// </summary>
    public (bool ShouldBlockClient, ThrottleInfo? ThrottleInfo) ShouldBanIp(AntiDosFirewallRequestInfo requestInfo)
    {
        if (requestInfo == null)
        {
            throw new ArgumentNullException(nameof(requestInfo));
        }

        if (_antiDosConfig.Value.BannedIPAddressRanges?.Any() != true)
        {
            return (false, null);
        }

        if (requestInfo.IP == null)
        {
            return (false, null);
        }

        var iPAddress = IPAddress.Parse(requestInfo.IP);

        foreach (var range in _antiDosConfig.Value.BannedIPAddressRanges)
        {
            var ipRange = IPAddressRange.Parse(range);
            if (ipRange.Contains(iPAddress))
            {
                return (true, new ThrottleInfo
                              {
                                  ExpiresAt = GetCacheExpiresAt(),
                                  RequestsCount = _antiDosConfig.Value.AllowedRequests,
                                  BanReason = $"IP: {requestInfo.IP} is in the `{range}` range.",
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
            clientThrottleInfo = new ThrottleInfo { RequestsCount = 1, ExpiresAt = expiresAt };
            _cacheService.Add(key, clientThrottleInfo, expiresAt, 1);
            return (false, clientThrottleInfo);
        }

        if (clientThrottleInfo.RequestsCount > _antiDosConfig.Value.AllowedRequests)
        {
            clientThrottleInfo.BanReason = "IsDosAttack";
            _cacheService.Add(key, clientThrottleInfo, expiresAt, 1);
            return (true, clientThrottleInfo);
        }

        clientThrottleInfo.RequestsCount++;
        _cacheService.Add(key, clientThrottleInfo, expiresAt, 1);
        return (false, clientThrottleInfo);
    }

    /// <summary>
    ///     Returns cache's expirations date
    /// </summary>
    public DateTimeOffset GetCacheExpiresAt() => DateTimeOffset.UtcNow.AddMinutes(_antiDosConfig.Value.DurationMin);

    /// <summary>
    ///     Returns cache's key
    /// </summary>
    public string GetCacheKey(AntiDosFirewallRequestInfo requestInfo)
    {
        if (requestInfo == null)
        {
            throw new ArgumentNullException(nameof(requestInfo));
        }

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

        if (throttleInfo.IsLogged)
        {
            return;
        }

        _logger.LogWarning($"Banned IP: {requestInfo.IP}, UserAgent: {requestInfo.UserAgent}. {throttleInfo}");
        throttleInfo.IsLogged = true;
        _cacheService.Add(GetCacheKey(requestInfo), throttleInfo, GetCacheExpiresAt(), 1);
    }
}