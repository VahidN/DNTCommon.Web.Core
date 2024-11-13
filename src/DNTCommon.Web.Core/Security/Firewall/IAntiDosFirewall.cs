namespace DNTCommon.Web.Core;

/// <summary>
///     Anti Dos Firewall Service
/// </summary>
public interface IAntiDosFirewall
{
    /// <summary>
    ///     Such as `google` or `bing`.
    /// </summary>
    bool IsGoodBot(AntiDosFirewallRequestInfo requestInfo);

    /// <summary>
    ///     Such as `asafaweb`.
    /// </summary>
    (bool ShouldBlockClient, ThrottleInfo? ThrottleInfo) IsBadBot(AntiDosFirewallRequestInfo requestInfo);

    /// <summary>
    ///     Is from remote localhost
    /// </summary>
    bool? IsFromRemoteLocalhost(AntiDosFirewallRequestInfo requestInfo);

    /// <summary>
    ///     Such as `HTTP_ACUNETIX_PRODUCT`.
    /// </summary>
    (bool ShouldBlockClient, ThrottleInfo? ThrottleInfo) HasUrlAttackVectors(AntiDosFirewallRequestInfo requestInfo);

    /// <summary>
    ///     Such as `HTTP_ACUNETIX_PRODUCT`.
    /// </summary>
    (bool ShouldBlockClient, ThrottleInfo? ThrottleInfo) ShouldBanBotHeaders(AntiDosFirewallRequestInfo requestInfo);

    /// <summary>
    ///     Such as `asafaweb`.
    /// </summary>
    (bool ShouldBlockClient, ThrottleInfo? ThrottleInfo) ShouldBanUserAgent(AntiDosFirewallRequestInfo requestInfo);

    /// <summary>
    ///     Should block client based on its info?
    /// </summary>
    (bool ShouldBlockClient, ThrottleInfo? ThrottleInfo) ShouldBlockClient(AntiDosFirewallRequestInfo requestInfo);

    /// <summary>
    ///     Should block client based on its IP?
    /// </summary>
    (bool ShouldBlockClient, ThrottleInfo? ThrottleInfo) ShouldBanIp(AntiDosFirewallRequestInfo requestInfo);

    /// <summary>
    ///     Is this a dos attack?
    /// </summary>
    (bool ShouldBlockClient, ThrottleInfo? ThrottleInfo) IsDosAttack(AntiDosFirewallRequestInfo requestInfo);

    /// <summary>
    ///     Returns cache's expirations date
    /// </summary>
    DateTimeOffset GetCacheExpiresAt();

    /// <summary>
    ///     Returns cache's key
    /// </summary>
    string GetCacheKey(AntiDosFirewallRequestInfo requestInfo);

    /// <summary>
    ///     Logs a warning
    /// </summary>
    void LogIt(ThrottleInfo? throttleInfo, AntiDosFirewallRequestInfo? requestInfo);
}