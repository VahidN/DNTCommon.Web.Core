using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DNTCommon.Web.Core
{
    /// <summary>
    /// Des CryptoProvider Service Extensions
    /// </summary>
    public static class DesCryptoProviderServiceExtensions
    {
        /// <summary>
        /// Adds IDesCryptoProvider to IServiceCollection.
        /// </summary>
        public static IServiceCollection AddDesProviderService(this IServiceCollection services)
        {
            services.AddSingleton<IDesCryptoProvider, DesCryptoProvider>();
            return services;
        }
    }

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

    /// <summary>
    /// The DES protection provider
    /// </summary>
    public class DesCryptoProvider : IDesCryptoProvider
    {
        private readonly ILogger<DesCryptoProvider> _logger;
        private readonly ISerializationProvider _serializationProvider;

        /// <summary>
        /// The DES protection provider
        /// </summary>
        public DesCryptoProvider(
            ILogger<DesCryptoProvider> logger,
            ISerializationProvider serializationProvider)
        {
            _logger = logger;
            _serializationProvider = serializationProvider;
        }

        /// <summary>
        /// Creates the hash of the message
        /// </summary>
        public (string HashString, byte[] HashBytes) Hash(string inputText)
        {
            using (var sha = SHA256.Create())
            {
                var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(inputText));
                return (Encoding.UTF8.GetString(hash), hash);
            }
        }

        /// <summary>
        /// Decrypts the message
        /// </summary>
        public string? Decrypt(string inputText, string key)
        {
            if (string.IsNullOrWhiteSpace(inputText))
            {
                throw new ArgumentNullException(nameof(inputText));
            }

            try
            {
                var inputBytes = WebEncoders.Base64UrlDecode(inputText);
                var bytes = decrypt(inputBytes, key);
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
        public string Encrypt(string inputText, string key)
        {
            if (string.IsNullOrWhiteSpace(inputText))
            {
                throw new ArgumentNullException(nameof(inputText));
            }

            var inputBytes = Encoding.UTF8.GetBytes(inputText);
            var bytes = encrypt(inputBytes, key);
            return WebEncoders.Base64UrlEncode(bytes);
        }

        /// <summary>
        /// It will serialize an object as a JSON string and then encrypt it as a Base64UrlEncode string.
        /// </summary>
        public string EncryptObject(object data, string key)
        {
            return Encrypt(_serializationProvider.Serialize(data), key);
        }

        /// <summary>
        /// It will decrypt a Base64UrlEncode encrypted JSON string and then deserialize it as an object.
        /// </summary>
        public T? DecryptObject<T>(string data, string key)
        {
            var decryptedData = Decrypt(data, key);
            return decryptedData == null ? default : _serializationProvider.Deserialize<T>(decryptedData);
        }

        [SuppressMessage("Microsoft.Usage", "CA5350:encrypt uses a weak cryptographic algorithm TripleDES",
                        Justification = "That's enough for our usecase!")]
        private byte[] encrypt(byte[] data, string key)
        {
            var desKey = getDesKey(key);
            using (var des = new TripleDESCryptoServiceProvider
            {
                Key = desKey,
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7
            })
            {
                using var encryptor = des.CreateEncryptor();
                using var cipherStream = new MemoryStream();
                using (var cryptoStream = new CryptoStream(cipherStream, encryptor, CryptoStreamMode.Write))
                using (var binaryWriter = new BinaryWriter(cryptoStream))
                {
                    // prepend IV to data
                    cipherStream.Write(des.IV); // This is an auto-generated random key
                    binaryWriter.Write(data);
                    cryptoStream.FlushFinalBlock();
                }
                return cipherStream.ToArray();
            }
        }

        [SuppressMessage("Microsoft.Usage", "CA5350:encrypt uses a weak cryptographic algorithm TripleDES",
                        Justification = "That's enough for our usecase!")]
        private byte[] decrypt(byte[] data, string key)
        {
            var desKey = getDesKey(key);
            using (var des = new TripleDESCryptoServiceProvider
            {
                Key = desKey,
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7
            })
            {
                var iv = new byte[8]; // 3DES-IV is always 8 bytes/64 bits because block size is always 64 bits
                Array.Copy(data, 0, iv, 0, iv.Length);

                using var ms = new MemoryStream();
                using (var decryptor = new CryptoStream(ms, des.CreateDecryptor(desKey, iv), CryptoStreamMode.Write))
                using (var binaryWriter = new BinaryWriter(decryptor))
                {
                    // decrypt cipher text from data, starting just past the IV
                    binaryWriter.Write(
                        data,
                        iv.Length,
                        data.Length - iv.Length
                    );
                }
                return ms.ToArray();
            }
        }

        private byte[] getDesKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key));
            }
            // The key size of TripleDES is 168 bits, its len in byte is 24 Bytes (or 192 bits).
            // Last bit of each byte is not used (or used as version in some hardware).
            // Key len for TripleDES can also be 112 bits which is again stored in 128 bits or 16 bytes.
            var hashBytes = Hash(key).HashBytes;
            return hashBytes.Take(24).ToArray();
        }
    }
}