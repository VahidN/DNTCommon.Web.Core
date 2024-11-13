namespace DNTCommon.Web.Core;

/// <summary>
///     Delay Delivery
/// </summary>
public class DelayDelivery
{
    /// <summary>
    ///     Its default value is 1 second.
    /// </summary>
    public TimeSpan Delay { set; get; } = TimeSpan.FromSeconds(value: 1);

    /// <summary>
    ///     Its default value is after sending 30 messages.
    /// </summary>
    public int NumberOfMessages { set; get; } = 30;
}