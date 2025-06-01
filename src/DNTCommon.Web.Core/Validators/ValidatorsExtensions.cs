using System.Text;
using Microsoft.AspNetCore.Http;

namespace DNTCommon.Web.Core;

/// <summary>
///     Validators Extensions
/// </summary>
public static class ValidatorsExtensions
{
#if !NET_6
    /// <summary>
    ///     Tries to parse a string into a Guid-value.
    /// </summary>
    public static bool IsValidGuid(this string? value)
        => !value.IsEmpty() && Guid.TryParse(value, CultureInfo.InvariantCulture, out _);
#endif

    public static (bool Success, string? ErrorMessage) IsValidIFormFile(this object? value,
        Func<IFormFile, (bool Success, string? ErrorMessage)> isValidFile,
        string? defaultError)
    {
        ArgumentNullException.ThrowIfNull(isValidFile);

        return value switch
        {
            IFormFile file => AreValidFiles(isValidFile, file),
            IList<IFormFile> files => AreValidFiles(isValidFile, files),
            IFormFileCollection fileCollection => AreValidFiles(isValidFile, fileCollection),
            _ => (false, defaultError)
        };
    }

    private static (bool Success, string? ErrorMessage) AreValidFiles(
        Func<IFormFile, (bool Success, string? ErrorMessage)> isValidFile,
        params IEnumerable<IFormFile> files)
    {
        foreach (var file in files)
        {
            var result = isValidFile(file);

            if (!result.Success)
            {
                return result;
            }
        }

        return (true, null);
    }

    /// <summary>
    ///     Determines whether the given file is a text file
    /// </summary>
    public static bool IsLikelyTextFile(this string? filePath)
    {
        try
        {
            if (filePath.IsEmpty())
            {
                return false;
            }

            var fi = new FileInfo(filePath);

            if (fi.Length == 0)
            {
                return true;
            }

            var buffer = File.ReadAllBytes(filePath);

            // Null byte heuristic
            if (buffer.Contains((byte)0x00))
            {
                return false;
            }

            // UTF-8 check
            try
            {
                _ = Encoding.UTF8.GetString(buffer);
            }
            catch
            {
                // Fallback: if most bytes are printable ASCII, consider text
                var textBytes = buffer.Count(b => b is >= 32 and <= 126 or 9 or 10 or 13);

                return (double)textBytes / buffer.Length > 0.75;
            }

            return true;
        }
        catch
        {
            return false;
        }
    }
}
