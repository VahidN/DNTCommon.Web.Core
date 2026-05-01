using System.Text;

namespace DNTCommon.Web.Core;

/// <summary>
///     A helper class to execute the Chrome's process
/// </summary>
public static class ExecuteApplicationProcessExtensions
{
    private static readonly TimeSpan DefaultWaitForExit = TimeSpan.FromMinutes(value: 3);

    /// <summary>
    ///     A helper method to execute a process
    /// </summary>
    public static async Task<ExecuteProcessInfo> ExecuteProcessAsync(this ApplicationStartInfo startInfo,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(startInfo);

        KillAllAppProcesses(startInfo);

        var output = new StringBuilder();

        using var process = startInfo.CreateProcess();
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
                await process.WaitForExitAsync(cancellationToken);
            }
        }
        finally
        {
            process.OutputDataReceived -= OnProcessOnOutputDataReceived;
            process.ErrorDataReceived -= OnProcessOnOutputDataReceived;
        }

        return GetProcessOutputData(output.ToString().Trim(), process);

        void OnProcessOnOutputDataReceived(object o, DataReceivedEventArgs e) => output.AppendLine(e.Data);
    }

    /// <summary>
    ///     A helper method to execute a process
    /// </summary>
    public static ExecuteProcessInfo ExecuteProcess(this ApplicationStartInfo startInfo)
    {
        ArgumentNullException.ThrowIfNull(startInfo);

        KillAllAppProcesses(startInfo);

        var output = new StringBuilder();

        using var process = startInfo.CreateProcess();
        process.OutputDataReceived += OnProcessOnOutputDataReceived;
        process.ErrorDataReceived += OnProcessOnOutputDataReceived;

        try
        {
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            process.WaitForExit((int)(startInfo.WaitForExit?.TotalMilliseconds ??
                                      DefaultWaitForExit.TotalMilliseconds));
        }
        finally
        {
            process.OutputDataReceived -= OnProcessOnOutputDataReceived;
            process.ErrorDataReceived -= OnProcessOnOutputDataReceived;
        }

        return GetProcessOutputData(output.ToString().Trim(), process);

        void OnProcessOnOutputDataReceived(object o, DataReceivedEventArgs e) => output.AppendLine(e.Data);
    }

    private static ExecuteProcessInfo GetProcessOutputData(string errorMessage, Process process)
    {
        var (exitCode, isExited) = GetProcessExitInfo(process);

        if (isExited)
        {
            return new ExecuteProcessInfo(exitCode, isExited, errorMessage);
        }

        KillThisProcess(process);

        return new ExecuteProcessInfo(exitCode, isExited, errorMessage);
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

    private static void KillAllAppProcesses(ApplicationStartInfo startInfo)
    {
        if (!startInfo.KillProcessOnStart || startInfo.ProcessName.IsEmpty())
        {
            return;
        }

        var processes = Process.GetProcessesByName(startInfo.ProcessName);

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

    private static Process CreateProcess(this ApplicationStartInfo startInfo)
    {
        var process = new Process();

        if (!startInfo.AppPath.IsEmpty())
        {
            process.StartInfo.FileName = startInfo.AppPath;
        }

        if (!startInfo.Arguments.IsEmpty())
        {
            process.StartInfo.Arguments = startInfo.Arguments;
        }

        if (!startInfo.ArgumentsList.IsNullOrEmpty())
        {
            foreach (var arg in startInfo.ArgumentsList)
            {
                process.StartInfo.ArgumentList.Add(arg);
            }
        }

        process.StartInfo.UseShellExecute = false;
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.RedirectStandardInput = true;

        return process;
    }
}
