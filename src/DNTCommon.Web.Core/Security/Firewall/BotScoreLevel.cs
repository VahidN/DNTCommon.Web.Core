namespace DNTCommon.Web.Core;

/// <summary>
///     Bot Score Level
/// </summary>
public enum BotScoreLevel
{
    /// <summary>
    ///     0–20
    /// </summary>
    Human = 0,

    /// <summary>
    ///     20–40
    /// </summary>
    Suspicious = 1,

    /// <summary>
    ///     40–60
    /// </summary>
    LikelyBot = 2,

    /// <summary>
    ///     60–80
    /// </summary>
    Bot = 3,

    /// <summary>
    ///     80–100
    /// </summary>
    HighRiskBot = 4
}
