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

        var derivedBytes =
            Rfc2898DeriveBytes.Pbkdf2(password, saltBytes, Iterations, HashAlgorithmName.SHA512, outputLength: 48);

        var keyBytes = new byte[32];
        var ivBytes = new byte[16];
        Array.Copy(derivedBytes, sourceIndex: 0, keyBytes, destinationIndex: 0, length: 32);
        Array.Copy(derivedBytes, sourceIndex: 32, ivBytes, destinationIndex: 0, length: 16);

        aes.Key = keyBytes;
        aes.IV = ivBytes;

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

        var derivedBytes =
            Rfc2898DeriveBytes.Pbkdf2(password, saltBytes, Iterations, HashAlgorithmName.SHA512, outputLength: 48);

        var keyBytes = new byte[32];
        var ivBytes = new byte[16];
        Array.Copy(derivedBytes, sourceIndex: 0, keyBytes, destinationIndex: 0, length: 32);
        Array.Copy(derivedBytes, sourceIndex: 32, ivBytes, destinationIndex: 0, length: 16);

        aes.Key = keyBytes;
        aes.IV = ivBytes;

        var buffer = WebEncoders.Base64UrlDecode(cipherText);

        var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

        using var ms = new MemoryStream(buffer);
        using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        using var sr = new StreamReader(cs);

        return sr.ReadToEnd();
    }
}
