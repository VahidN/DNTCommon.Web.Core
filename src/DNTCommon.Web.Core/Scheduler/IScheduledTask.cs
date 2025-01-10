namespace DNTCommon.Web.Core;

/// <summary>
///     Scheduled task's contract.
/// </summary>
public interface IScheduledTask
{
    /// <summary>
    ///     Scheduled task's logic.
    /// </summary>
    Task RunAsync(CancellationToken cancellationToken);
}