namespace DNTCommon.Web.Core;

/// <summary>
///     Add it as services.AddSingleton(IProtectionProvider, ProtectionProvider)
///     More info: http://www.dntips.ir/post/2519
/// </summary>
public interface IProtectionProviderService
{
    /// <summary>
    ///     Decrypts the message
    /// </summary>
    string? Decrypt(string? inputText);

    /// <summary>
    ///     Encrypts the message
    /// </summary>
    string? Encrypt([NotNullIfNotNull(nameof(inputText))] string? inputText);

    /// <summary>
    ///     It will serialize an object as a JSON string and then encrypt it as a Base64UrlEncode string.
    /// </summary>
    string? EncryptObject([NotNullIfNotNull(nameof(data))] object? data);

    /// <summary>
    ///     It will decrypt a Base64UrlEncode encrypted JSON string and then deserialize it as an object.
    /// </summary>
    T? DecryptObject<T>(string? data);
}