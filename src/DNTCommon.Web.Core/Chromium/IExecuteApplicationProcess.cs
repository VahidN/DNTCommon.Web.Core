namespace DNTCommon.Web.Core;

/// <summary>
///     A helper utility to execute a process
/// </summary>
public interface IExecuteApplicationProcess
{
    /// <summary>
    ///     A helper method to execute a process
    /// </summary>
    Task<string> ExecuteProcessAsync(ApplicationStartInfo startInfo, CancellationToken cancellationToken = default);

    /// <summary>
    ///     A helper method to execute a process
    /// </summary>
    string ExecuteProcess(ApplicationStartInfo startInfo);
}
