using System.Text;

namespace DNTCommon.Web.Core;

/// <summary>
///     Resolves the stack back to the C# source format of the calls (and is an inspectable list of stack frames)
/// </summary>
public class EnhancedStackTraceService : IEnhancedStackTraceService
{
    /// <summary>
    ///     Resolves the stack back to the C# source format of the calls (and is an inspectable list of stack frames)
    /// </summary>
    public IEnumerable<StackFrameInfo> GetCurrentStackFrames(Predicate<Type> skipFrame)
    {
        ArgumentNullException.ThrowIfNull(skipFrame);

        return GetStackFrames(skipFrame);
    }

    /// <summary>
    ///     Resolves the stack back to the C# source format of the calls (and is an inspectable list of stack frames)
    /// </summary>
    public string GetCurrentStackTrace(Predicate<Type> skipFrame)
    {
        ArgumentNullException.ThrowIfNull(skipFrame);

        var sb = new StringBuilder();

        foreach (var frame in GetStackFrames(skipFrame))
        {
            if (!string.IsNullOrEmpty(frame.File))
            {
                sb.Append(frame.File);
                var lineNo = frame.Line;
                var colNo = frame.Column;

                if (lineNo.HasValue && colNo.HasValue)
                {
                    sb.Append(value: '(').Append(lineNo).Append(value: ',').Append(colNo).Append(value: "): ");
                }
            }

            sb.Append(value: "at ").AppendLine(frame.Method);
        }

        return sb.ToString();
    }

    private static IEnumerable<StackFrameInfo> GetStackFrames(Predicate<Type> skipFrame)
    {
        var enhancedStackTrace = EnhancedStackTrace.Current();
        var stackFrames = enhancedStackTrace.GetFrames();

        if (stackFrames is null)
        {
            yield break;
        }

        foreach (var stackFrame in stackFrames)
        {
            if (!stackFrame.HasMethod())
            {
                continue;
            }

            var methodBase = stackFrame.GetMethod();

            if (methodBase is null)
            {
                continue;
            }

            var resolvedMethod = EnhancedStackTrace.GetMethodDisplayString(methodBase);

            if (resolvedMethod is null)
            {
                continue;
            }

            var declaringType = methodBase.DeclaringType;

            if (declaringType is null)
            {
                continue;
            }

            if (declaringType == typeof(EnhancedStackTraceService))
            {
                continue;
            }

            if (skipFrame(declaringType))
            {
                continue;
            }

            yield return new StackFrameInfo
            {
                Method = resolvedMethod.ToString(),
                File = stackFrame.GetFileName(),
                Line = stackFrame.GetFileLineNumber(),
                Column = stackFrame.GetFileColumnNumber()
            };
        }
    }
}
