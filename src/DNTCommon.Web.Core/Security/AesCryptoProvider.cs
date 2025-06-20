using System.Text;
using Microsoft.AspNetCore.WebUtilities;

namespace DNTCommon.Web.Core;

/// <summary>
///     The AES protection provider
/// </summary>
public static class AesCryptoProvider
{
    private const int Iterations = 210_000;

    /// <summary>
    ///     Encrypts the message using the Advanced Encryption Standard (AES)
    /// </summary>
    public static string AesEncrypt(this string plainText, string password, string salt)
    {
        ArgumentNullException.ThrowIfNull(plainText);
        ArgumentNullException.ThrowIfNull(password);
        ArgumentNullException.ThrowIfNull(salt);

        using var aes = Aes.Create();

        var saltBytes = Encoding.UTF8.GetBytes(salt);
        using var key = new Rfc2898DeriveBytes(password, saltBytes, Iterations, HashAlgorithmName.SHA512);
        aes.Key = key.GetBytes(cb: 32);
        aes.IV = key.GetBytes(cb: 16);

        var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

        using var ms = new MemoryStream();

        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
        using (var sw = new StreamWriter(cs))
        {
            sw.Write(plainText);
        }

        return WebEncoders.Base64UrlEncode(ms.ToArray());
    }

    /// <summary>
    ///     Decrypts the message using the Advanced Encryption Standard (AES)
    /// </summary>
    public static string AesDecrypt(this string cipherText, string password, string salt)
    {
        ArgumentNullException.ThrowIfNull(cipherText);
        ArgumentNullException.ThrowIfNull(password);
        ArgumentNullException.ThrowIfNull(salt);

        using var aes = Aes.Create();

        var saltBytes = Encoding.UTF8.GetBytes(salt);
        using var key = new Rfc2898DeriveBytes(password, saltBytes, Iterations, HashAlgorithmName.SHA512);
        aes.Key = key.GetBytes(cb: 32);
        aes.IV = key.GetBytes(cb: 16);

        var buffer = WebEncoders.Base64UrlDecode(cipherText);

        var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

        using var ms = new MemoryStream(buffer);
        using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        using var sr = new StreamReader(cs);

        return sr.ReadToEnd();
    }
}
