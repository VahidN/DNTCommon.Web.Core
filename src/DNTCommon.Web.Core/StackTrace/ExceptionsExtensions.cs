using System.Text;

namespace DNTCommon.Web.Core;

/// <summary>
///     Exceptions utils
/// </summary>
public static class ExceptionsExtensions
{
    /// <summary>
    ///     Extracts all the InnerExceptions
    /// </summary>
    /// <param name="ex"></param>
    /// <returns></returns>
    public static string FormatException(this Exception? ex)
    {
        if (ex is null)
        {
            return string.Empty;
        }

        ex = ex.Demystify();

        var exError = new StringBuilder();

        while (ex is not null)
        {
            exError.AppendLine(ex.Message);
            ex = ex.InnerException;
        }

        return exError.ToString();
    }
}
