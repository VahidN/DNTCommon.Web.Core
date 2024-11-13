namespace DNTCommon.Web.Core;

/// <summary>
///     Resolves the stack back to the C# source format of the calls (and is an inspectable list of stack frames)
/// </summary>
public interface IEnhancedStackTraceService
{
    /// <summary>
    ///     Resolves the stack back to the C# source format of the calls (and is an inspectable list of stack frames)
    /// </summary>
    IEnumerable<StackFrameInfo> GetCurrentStackFrames(Predicate<Type> skipFrame);

    /// <summary>
    ///     Resolves the stack back to the C# source format of the calls (and is an inspectable list of stack frames)
    /// </summary>
    string GetCurrentStackTrace(Predicate<Type> skipFrame);
}