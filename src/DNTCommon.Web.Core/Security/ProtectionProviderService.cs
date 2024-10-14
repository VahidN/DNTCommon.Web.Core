using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;

namespace DNTCommon.Web.Core;

/// <summary>
///     Protection Provider Service
/// </summary>
public class ProtectionProviderService : IProtectionProviderService
{
    private readonly IDataProtector _dataProtector;
    private readonly ILogger<ProtectionProviderService> _logger;
    private readonly ISerializationProvider _serializationProvider;

    /// <summary>
    ///     The default protection provider
    /// </summary>
    public ProtectionProviderService(IDataProtectionProvider dataProtectionProvider,
        ILogger<ProtectionProviderService> logger,
        ISerializationProvider serializationProvider)
    {
        ArgumentNullException.ThrowIfNull(dataProtectionProvider);
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _dataProtector =
            dataProtectionProvider.CreateProtector(typeof(ProtectionProviderService).FullName ??
                                                   nameof(ProtectionProviderService));

        _serializationProvider =
            serializationProvider ?? throw new ArgumentNullException(nameof(serializationProvider));
    }

    /// <summary>
    ///     Decrypts the message
    /// </summary>
    public string? Decrypt(string? inputText)
    {
        if (inputText is null)
        {
            return null;
        }

        try
        {
            return _dataProtector.Unprotect(inputText);
        }
        catch (FormatException ex)
        {
            _logger.LogError(ex.Demystify(), message: "Invalid base 64 string. Fall through.");
        }
        catch (CryptographicException ex)
        {
            _logger.LogError(ex.Demystify(), message: "Invalid protected payload. Fall through.");
        }

        return null;
    }

    /// <summary>
    ///     Encrypts the message
    /// </summary>
    public string? Encrypt([NotNullIfNotNull(nameof(inputText))] string? inputText)
        => inputText is null ? null : _dataProtector.Protect(inputText);

    /// <summary>
    ///     It will serialize an object as a JSON string and then encrypt it as a Base64UrlEncode string.
    /// </summary>
    public string? EncryptObject([NotNullIfNotNull(nameof(data))] object? data)
        => data is null ? null : Encrypt(_serializationProvider.Serialize(data));

    /// <summary>
    ///     It will decrypt a Base64UrlEncode encrypted JSON string and then deserialize it as an object.
    /// </summary>
    public T? DecryptObject<T>(string? data)
    {
        var decryptData = Decrypt(data);

        return decryptData == null ? default : _serializationProvider.Deserialize<T>(decryptData);
    }
}