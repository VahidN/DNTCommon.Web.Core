using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DNTCommon.Web.Core
{
    /// <summary>
    /// More info: http://www.dotnettips.info/post/2519
    /// </summary>
    public static class ProtectionProviderServiceExtensions
    {
        /// <summary>
        /// Adds IProtectionProviderService to IServiceCollection.
        /// </summary>
        public static IServiceCollection AddProtectionProviderService(this IServiceCollection services)
        {
            services.AddSingleton<IProtectionProviderService, ProtectionProviderService>();
            return services;
        }
    }

    /// <summary>
    /// Add it as services.AddSingleton(IProtectionProvider, ProtectionProvider)
    /// More info: http://www.dotnettips.info/post/2519
    /// </summary>
    public interface IProtectionProviderService
    {
        /// <summary>
        /// Decrypts the message
        /// </summary>
        string Decrypt(string inputText);

        /// <summary>
        /// Encrypts the message
        /// </summary>
        string Encrypt(string inputText);

        /// <summary>
        /// It will serialize an object as a JSON string and then encrypt it as a Base64UrlEncode string.
        /// </summary>
        string EncryptObject(object data);

        /// <summary>
        /// It will decrypt a Base64UrlEncode encrypted JSON string and then deserialize it as an object.
        /// </summary>
        T DecryptObject<T>(string data);
    }

    /// <summary>
    /// Protection Provider Service
    /// </summary>
    public class ProtectionProviderService : IProtectionProviderService
    {
        private readonly ILogger<ProtectionProviderService> _logger;
        private readonly IDataProtector _dataProtector;
        private readonly ISerializationProvider _serializationProvider;

        /// <summary>
        /// The default protection provider
        /// </summary>
        public ProtectionProviderService(
            IDataProtectionProvider dataProtectionProvider,
            ILogger<ProtectionProviderService> logger,
            ISerializationProvider serializationProvider)
        {
            if (dataProtectionProvider == null)
            {
                throw new ArgumentNullException(nameof(dataProtectionProvider));
            }
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _dataProtector = dataProtectionProvider.CreateProtector(typeof(ProtectionProviderService).FullName);
            _serializationProvider = serializationProvider ?? throw new ArgumentNullException(nameof(serializationProvider));
        }

        /// <summary>
        /// Decrypts the message
        /// </summary>
        public string Decrypt(string inputText)
        {
            if (inputText == null)
            {
                throw new ArgumentNullException(nameof(inputText));
            }

            try
            {
                var inputBytes = WebEncoders.Base64UrlDecode(inputText);
                var bytes = _dataProtector.Unprotect(inputBytes);
                return Encoding.UTF8.GetString(bytes);
            }
            catch (FormatException ex)
            {
                _logger.LogError(ex.Message, "Invalid base 64 string. Fall through.");
            }
            catch (CryptographicException ex)
            {
                _logger.LogError(ex.Message, "Invalid protected payload. Fall through.");
            }

            return null;
        }

        /// <summary>
        /// Encrypts the message
        /// </summary>
        public string Encrypt(string inputText)
        {
            if (inputText == null)
            {
                throw new ArgumentNullException(nameof(inputText));
            }

            var inputBytes = Encoding.UTF8.GetBytes(inputText);
            var bytes = _dataProtector.Protect(inputBytes);
            return WebEncoders.Base64UrlEncode(bytes);
        }

        /// <summary>
        /// It will serialize an object as a JSON string and then encrypt it as a Base64UrlEncode string.
        /// </summary>
        public string EncryptObject(object data)
        {
            return Encrypt(_serializationProvider.Serialize(data));
        }

        /// <summary>
        /// It will decrypt a Base64UrlEncode encrypted JSON string and then deserialize it as an object.
        /// </summary>
        public T DecryptObject<T>(string data)
        {
            return _serializationProvider.Deserialize<T>(Decrypt(data));
        }
    }
}