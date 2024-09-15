using System.Text;

namespace DNTCommon.Web.Core;

/// <summary>
///     A helper class to execute the Chrome's process
/// </summary>
public class ExecuteApplicationProcess : IExecuteApplicationProcess
{
    /// <summary>
    ///     A helper method to execute the Chrome's process
    /// </summary>
    public async Task<string> ExecuteProcessAsync(string processName,
        string arguments,
        string converterExecutionPath,
        TimeSpan waitForExit)
    {
        KillAllConverterProcesses(processName);

        var output = new StringBuilder();
        var waitToClose = TimeSpan.FromSeconds(value: 10);

        using var process = new Process();
        process.StartInfo.FileName = converterExecutionPath;
        process.StartInfo.Arguments = arguments;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.RedirectStandardInput = true;
        process.OutputDataReceived += OnProcessOnOutputDataReceived;
        process.ErrorDataReceived += OnProcessOnOutputDataReceived;

        try
        {
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            using CancellationTokenSource cancellationTokenSource = new();
            cancellationTokenSource.CancelAfter(waitForExit);
            await process.WaitForExitAsync(cancellationTokenSource.Token);
        }
        finally
        {
            process.OutputDataReceived -= OnProcessOnOutputDataReceived;
            process.ErrorDataReceived -= OnProcessOnOutputDataReceived;
        }

        var errorMessage = output.ToString().Trim();

        var (exitCode, isExited) = GetProcessExitInfo(process);

        if (exitCode == 0 && isExited)
        {
            return errorMessage;
        }

        await Task.Delay(waitToClose);
        KillAllConverterProcesses(processName);

        throw new InvalidOperationException(Invariant($"HasExited: {isExited}, ExitCode: {exitCode}, {errorMessage}"));

        void OnProcessOnOutputDataReceived(object o, DataReceivedEventArgs e) => output.AppendLine(e.Data);
    }

    private static void KillAllConverterProcesses(string processName)
    {
        var chromeProcesses = Process.GetProcessesByName(processName);

        foreach (var chromeProcess in chromeProcesses)
        {
            try
            {
                chromeProcess.Kill(entireProcessTree: true);
            }
            catch
            {
                // It's not important!
            }
        }
    }

    private static (int ExitCode, bool IsExited) GetProcessExitInfo(Process process)
    {
        try
        {
            return (process.ExitCode, process.HasExited);
        }
        catch
        {
            // It's not important!
        }

        return (-1, false);
    }
}