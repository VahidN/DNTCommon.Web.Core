using System.Text;

namespace DNTCommon.Web.Core;

/// <summary>
///     A helper class to execute the Chrome's process
/// </summary>
public class ExecuteApplicationProcess : IExecuteApplicationProcess
{
    private readonly TimeSpan _defaultWaitForExit = TimeSpan.FromMinutes(value: 3);

    /// <summary>
    ///     A helper method to execute a process
    /// </summary>
    public async Task<string> ExecuteProcessAsync(ApplicationStartInfo startInfo)
    {
        ArgumentNullException.ThrowIfNull(startInfo);

        if (startInfo.KillProcessOnStart && !startInfo.ProcessName.IsEmpty())
        {
            KillAllAppProcesses(startInfo.ProcessName);
        }

        var output = new StringBuilder();

        using var process = new Process();

        process.StartInfo.FileName =
            startInfo.AppPath ?? throw new InvalidOperationException(message: "AppPath is empty.");

        if (!startInfo.Arguments.IsEmpty())
        {
            process.StartInfo.Arguments = startInfo.Arguments;
        }

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

            if (startInfo.WaitForExit.HasValue)
            {
                using var cts = startInfo.WaitForExit.Value.ToCancellationTokenSource();
                await process.WaitForExitAsync(cts.Token);
            }
            else
            {
                await process.WaitForExitAsync();
            }
        }
        finally
        {
            process.OutputDataReceived -= OnProcessOnOutputDataReceived;
            process.ErrorDataReceived -= OnProcessOnOutputDataReceived;
        }

        var errorMessage = output.ToString().Trim();

        var (exitCode, isExited) = GetProcessExitInfo(process);

        if (isExited)
        {
            return errorMessage;
        }

        await Task.Delay(startInfo.WaitForExit ?? _defaultWaitForExit);
        KillThisProcess(process);

        return string.Create(CultureInfo.InvariantCulture,
            $"{errorMessage}{Environment.NewLine}HasExited: {isExited}, ExitCode: {exitCode}");

        void OnProcessOnOutputDataReceived(object o, DataReceivedEventArgs e) => output.AppendLine(e.Data);
    }

    private static void KillThisProcess(Process process)
    {
        try
        {
            process.Kill(entireProcessTree: true);
        }
        catch
        {
            // It's not important!
        }
    }

    private static void KillAllAppProcesses(string? processName)
    {
        if (processName.IsEmpty())
        {
            return;
        }

        var processes = Process.GetProcessesByName(processName);

        foreach (var process in processes)
        {
            KillThisProcess(process);
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
