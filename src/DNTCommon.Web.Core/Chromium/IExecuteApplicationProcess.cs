namespace DNTCommon.Web.Core;

/// <summary>
///     A helper utility to execute the Chrome's process
/// </summary>
public interface IExecuteApplicationProcess
{
    /// <summary>
    ///     A helper method to execute the Chrome's process
    /// </summary>
    Task<string> ExecuteProcessAsync(string processName,
        string arguments,
        string converterExecutionPath,
        TimeSpan waitForExit);
}