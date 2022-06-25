namespace DNTCommon.Web.Core;

/// <summary>
/// The DES protection provider
/// </summary>
public interface IDesCryptoProvider
{
    /// <summary>
    /// Decrypts the message
    /// </summary>
    string? Decrypt(string inputText, string key);

    /// <summary>
    /// It will decrypt a Base64UrlEncode encrypted JSON string and then deserialize it as an object.
    /// </summary>
    T? DecryptObject<T>(string data, string key);

    /// <summary>
    /// Encrypts the message
    /// </summary>
    string Encrypt(string inputText, string key);

    /// <summary>
    /// It will serialize an object as a JSON string and then encrypt it as a Base64UrlEncode string.
    /// </summary>
    string EncryptObject(object data, string key);

    /// <summary>
    /// Creates the hash of the message
    /// </summary>
    (string HashString, byte[] HashBytes) Hash(string inputText);
}