using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace DNTCommon.Web.Core
{
    /// <summary>
    /// Resolves the stack back to the C# source format of the calls (and is an inspectable list of stack frames)
    /// </summary>
    public class EnhancedStackTraceService : IEnhancedStackTraceService
    {
        /// <summary>
        /// Resolves the stack back to the C# source format of the calls (and is an inspectable list of stack frames)
        /// </summary>
        public IEnumerable<StackFrameInfo> GetCurrentStackFrames(Predicate<Type> skipFrame)
        {
            if (skipFrame == null)
            {
                throw new ArgumentNullException(nameof(skipFrame));
            }

            return getCurrentStackFrames(skipFrame);
        }

        /// <summary>
        /// Resolves the stack back to the C# source format of the calls (and is an inspectable list of stack frames)
        /// </summary>
        public string GetCurrentStackTrace(Predicate<Type> skipFrame)
        {
            if (skipFrame == null)
            {
                throw new ArgumentNullException(nameof(skipFrame));
            }

            var sb = new StringBuilder();
            foreach (var frame in getCurrentStackFrames(skipFrame))
            {
                if (!string.IsNullOrEmpty(frame.File))
                {
                    sb.Append(frame.File);
                    var lineNo = frame.Line;
                    var colNo = frame.Column;
                    if (lineNo.HasValue && colNo.HasValue)
                    {
                        sb.Append('(').Append(lineNo).Append(',').Append(colNo).Append("): ");
                    }
                }

                sb.Append("at ").AppendLine(frame.Method);
            }
            return sb.ToString();
        }

        private static IEnumerable<StackFrameInfo> getCurrentStackFrames(Predicate<Type> skipFrame)
        {
            var enhancedStackTrace = EnhancedStackTrace.Current();
            var stackFrames = enhancedStackTrace.GetFrames();
            if (stackFrames == null)
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
                if (methodBase == null)
                {
                    continue;
                }

                var resolvedMethod = EnhancedStackTrace.GetMethodDisplayString(methodBase);
                if (resolvedMethod == null)
                {
                    continue;
                }

                var declaringType = methodBase.DeclaringType;
                if (declaringType == null)
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
}