using System.Text;

namespace DNTCommon.Web.Core;

/// <summary>
///     PasswordHasher Service
/// </summary>
public class PasswordHasherService : IPasswordHasherService
{
    private const int SaltSize = 64;
    private const int KeySize = 64;
    private const int ExpectedHashedPasswordLength = SaltSize + KeySize;
    private const int Iterations = 210_000;
    private readonly HashAlgorithmName _algorithm = HashAlgorithmName.SHA512;
    private readonly RandomNumberGenerator _rand = RandomNumberGenerator.Create();

    /// <summary>
    ///     Creates a custom hash based on SHA256CryptoServiceProvider.
    /// </summary>
    public string GetSha256Hash(string input)
    {
        var byteValue = Encoding.UTF8.GetBytes(input);
        var byteHash = SHA256.HashData(byteValue);
        Array.Reverse(byteHash);
        return Convert.ToBase64String(byteHash);
    }

    /// <summary>
    ///     Creates a custom hash based on SHA1 CryptoServiceProvider.
    /// </summary>
    public string GetSha1Hash(string input)
    {
        var byteValue = Encoding.UTF8.GetBytes(input);
        var byteHash = SHA1.HashData(byteValue);
        return BitConverter.ToString(byteHash).Replace("-", "", StringComparison.OrdinalIgnoreCase).ToUpperInvariant();
    }

    /// <summary>
    ///     A cryptographic random number generator
    /// </summary>
    public Guid CreateCryptographicallySecureGuid() => new(GenerateRandomSalt());

    /// <summary>
    ///     A cryptographic random number generator
    /// </summary>
    /// <returns></returns>
    public byte[] GenerateRandomSalt()
    {
        var salt = new byte[16];
        _rand.GetBytes(salt);
        return salt;
    }

    /// <summary>
    ///     Computes a salted Pbkdf2 hash. Use IsValidPbkdf2Hash method to validate it.
    /// </summary>
    /// <param name="password"></param>
    /// <returns></returns>
    public string GetPbkdf2Hash(string password)
    {
        ArgumentNullException.ThrowIfNull(password);

        Span<byte> hashedPasswordBytes = stackalloc byte[ExpectedHashedPasswordLength];

        var saltBytes = hashedPasswordBytes[..SaltSize];
        var keyBytes = hashedPasswordBytes.Slice(SaltSize, KeySize);

        RandomNumberGenerator.Fill(saltBytes);
        Rfc2898DeriveBytes.Pbkdf2(password, saltBytes, keyBytes, Iterations, _algorithm);

        return Convert.ToBase64String(hashedPasswordBytes);
    }

    /// <summary>
    ///     Verifies a hash created by GetPbkdf2Hash method.
    /// </summary>
    /// <param name="hashedPassword"></param>
    /// <param name="providedPassword"></param>
    /// <returns></returns>
    public bool IsValidPbkdf2Hash(string hashedPassword, string providedPassword)
    {
        ArgumentNullException.ThrowIfNull(hashedPassword);
        ArgumentNullException.ThrowIfNull(providedPassword);

        var hashedPasswordByteCount = ComputeDecodedBase64ByteCount(hashedPassword);
        Span<byte> hashedPasswordBytes = stackalloc byte[hashedPasswordByteCount];

        if (!Convert.TryFromBase64String(hashedPassword, hashedPasswordBytes, out _))
        {
            return false;
        }

        if (hashedPasswordBytes.Length == 0)
        {
            return false;
        }

        if (hashedPasswordBytes.Length != ExpectedHashedPasswordLength)
        {
            return false;
        }

        var saltBytes = hashedPasswordBytes[..SaltSize];
        var expectedKeyBytes = hashedPasswordBytes.Slice(SaltSize, KeySize);

        Span<byte> actualKeyBytes = stackalloc byte[KeySize];
        Rfc2898DeriveBytes.Pbkdf2(providedPassword, saltBytes, actualKeyBytes, Iterations, _algorithm);

        return CryptographicOperations.FixedTimeEquals(expectedKeyBytes, actualKeyBytes);
    }

    private static int ComputeDecodedBase64ByteCount(string base64Str)
    {
        var characterCount = base64Str.Length;
        var paddingCount = 0;

        if (characterCount > 0 && base64Str[characterCount - 1] == '=')
        {
            paddingCount++;

            if (characterCount > 1 && base64Str[characterCount - 2] == '=')
            {
                paddingCount++;
            }
        }

        return characterCount * 3 / 4 - paddingCount;
    }
}