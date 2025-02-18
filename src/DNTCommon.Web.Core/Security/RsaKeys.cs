using System.Security.Cryptography.X509Certificates;
using Org.BouncyCastle.Security;

namespace DNTCommon.Web.Core;

/// <summary>
///     Creates and returns an XML string containing the key of the current RSA object.
/// </summary>
public class RsaKeys
{
    private RsaKeys()
    {
    }

    /// <summary>
    ///     Returns the public key
    /// </summary>
    public RSA? PublicKey { get; private set; }

    /// <summary>
    ///     Returns the private key
    /// </summary>
    public RSA? PrivateKey { get; private set; }

    /// <summary>
    ///     Returns the public key
    /// </summary>
    public string? PublicKeyXml { get; private set; }

    /// <summary>
    ///     Returns the private key
    /// </summary>
    public string? PrivateKeyXml { get; private set; }

    /// <summary>
    ///     Creates and returns an XML string containing the key of the current RSA object.
    /// </summary>
    /// <param name="dwKeySize">The size of the key to use in bits.</param>
    /// <returns></returns>
    public static RsaKeys CreateRandomKeyPair(int dwKeySize = 2048)
    {
        using var rsa = RSA.Create(dwKeySize);

        return new RsaKeys
        {
            PublicKeyXml = rsa.ToXmlString(includePrivateParameters: false),
            PrivateKeyXml = rsa.ToXmlString(includePrivateParameters: true)
        };
    }

    /// <summary>
    ///     Reads a .pfx file and extracts its public and private keys
    /// </summary>
    /// <param name="pfxFilePath">Certificate file's path</param>
    /// <param name="pfxPassword">Certificate file's password</param>
    /// <returns></returns>
    public static RsaKeys CreateKeyPairFromX509Certificate(string pfxFilePath, string pfxPassword)
    {
        using var certificate = GetX509Certificate2(pfxFilePath, pfxPassword);

        return new RsaKeys
        {
            PublicKey = certificate.GetRSAPublicKey() ??
                        throw new InvalidKeyException(message: "Failed to GetRSAPublicKey"),
            PrivateKey = certificate.GetRSAPrivateKey() ??
                         throw new InvalidKeyException(message: "Failed to GetRSAPrivateKey")
        };
    }

    private static X509Certificate2 GetX509Certificate2(string pfxFilePath, string pfxPassword)
    {
#if NET_8 || NET_7 || NET_6
        return new X509Certificate2(pfxFilePath, pfxPassword, X509KeyStorageFlags.EphemeralKeySet);
#else
        return X509CertificateLoader.LoadPkcs12FromFile(pfxFilePath, pfxPassword, X509KeyStorageFlags.EphemeralKeySet);
#endif
    }
}