using System.Text;

namespace DNTCommon.Web.Core;

/// <summary>
///     Md5 utils
/// </summary>
public static class Md5CryptoExtensions
{
    /// <summary>
    ///     Calculates the MD5 hash of a file.
    /// </summary>
    /// <param name="path">A relative or absolute path for the file that the current FileStream object will encapsulate.</param>
    /// <returns></returns>
    public static string FileMd5(this string path)
    {
        using var md5 = MD5.Create();
        using var file = new FileStream(path, FileMode.Open, FileAccess.Read);
        var retVal = md5.ComputeHash(file);

        return BitConverter.ToString(retVal).Replace("-", string.Empty, StringComparison.Ordinal); // hex string
    }

    /// <summary>
    ///     Calculates the MD5 hash of th input bytes
    /// </summary>
    /// <param name="inputBytes"></param>
    /// <returns></returns>
    public static string Md5Hash(this byte[] inputBytes)
    {
        // step 1, calculate MD5 hash from input
        var hash = MD5.HashData(inputBytes);

        // step 2, convert byte array to hex string
        var sb = new StringBuilder(2 * hash.Length);

        for (var i = 0; i < hash.Length; i++)
        {
            sb.AppendFormat(CultureInfo.InvariantCulture, "{0:X2}", hash[i]);
        }

        return sb.ToString();
    }

    /// <summary>
    ///     Calculates the MD5 hash of th input string
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string Md5Hash(this string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return string.Empty;
        }

        var data = Encoding.UTF8.GetBytes(input);

        return Md5Hash(data);
    }
}