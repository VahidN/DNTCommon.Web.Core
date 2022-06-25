namespace DNTCommon.Web.Core;

/// <summary>
/// StackFrame's Info
/// </summary>
public class StackFrameInfo
{
    /// <summary>
    /// Caller method
    /// </summary>
    public string Method { set; get; } = default!;

    /// <summary>
    /// Caller method's file
    /// </summary>
    public string? File { set; get; }

    /// <summary>
    /// Caller method's line
    /// </summary>
    public int? Line { set; get; }

    /// <summary>
    /// Caller method's column
    /// </summary>
    public int? Column { set; get; }
}