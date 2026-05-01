namespace DNTCommon.Web.Core;

public class ExecuteProcessInfo
{
    public ExecuteProcessInfo() { }

    public ExecuteProcessInfo(int exitCode, bool isExited, string? processOutput)
    {
        ExitCode = exitCode;
        IsExited = isExited;
        ProcessOutput = processOutput;
    }

    public int ExitCode { set; get; }

    public bool IsExited { set; get; }

    public string? ProcessOutput { set; get; }

    public void Deconstruct(out int exitCode, out bool isExited, out string? processOutput)
    {
        exitCode = ExitCode;
        isExited = IsExited;
        processOutput = ProcessOutput;
    }
}
