namespace DNTCommon.Web.Core;

/// <summary>
///     A helper class to execute the Chrome's process
/// </summary>
public class ExecuteApplicationProcess : IExecuteApplicationProcess
{
    /// <summary>
    ///     A helper method to execute a process
    /// </summary>
    public async Task<string> ExecuteProcessAsync(ApplicationStartInfo startInfo,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(startInfo);

        return await startInfo.ExecuteProcessAsync(cancellationToken);
    }

    public string ExecuteProcess(ApplicationStartInfo startInfo)
    {
        ArgumentNullException.ThrowIfNull(startInfo);

        return startInfo.ExecuteProcess();
    }
}
