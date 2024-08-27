using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace DNTCommon.Web.Core;

/// <summary>
///     PasswordHasher Service Extensions
/// </summary>
public static class PasswordHasherServiceExtensions
{
    private const int SaltSize = 64;
    private const int KeySize = 64;
    private const int ExpectedHashedPasswordLength = SaltSize + KeySize;
    private const int Iterations = 210_000;
    private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA512;

    /// <summary>
    ///     Adds IPasswordHasherService to IServiceCollection.
    /// </summary>
    public static IServiceCollection AddPasswordHasherService(this IServiceCollection services)
    {
        services.AddSingleton<IPasswordHasherService, PasswordHasherService>();

        return services;
    }

    /// <summary>
    ///     Creates a custom hash based on SHA256CryptoServiceProvider.
    /// </summary>
    public static string GetSha256Hash(this string input)
    {
        var byteValue = Encoding.UTF8.GetBytes(input);
        var byteHash = SHA256.HashData(byteValue);
        Array.Reverse(byteHash);

        return Convert.ToBase64String(byteHash);
    }

    /// <summary>
    ///     Creates a custom hash based on SHA1 CryptoServiceProvider.
    /// </summary>
    public static string GetSha1Hash(this string input)
    {
        var byteValue = Encoding.UTF8.GetBytes(input);
        var byteHash = SHA1.HashData(byteValue);

        return BitConverter.ToString(byteHash)
            .Replace(oldValue: "-", newValue: "", StringComparison.OrdinalIgnoreCase)
            .ToUpperInvariant();
    }

    /// <summary>
    ///     Computes a salted Pbkdf2 hash. Use IsValidPbkdf2Hash method to validate it.
    /// </summary>
    /// <param name="password"></param>
    /// <returns></returns>
    public static string GetPbkdf2Hash(this string password)
    {
        ArgumentNullException.ThrowIfNull(password);

        Span<byte> hashedPasswordBytes = stackalloc byte[ExpectedHashedPasswordLength];

        var saltBytes = hashedPasswordBytes[..SaltSize];
        var keyBytes = hashedPasswordBytes.Slice(SaltSize, KeySize);

        RandomNumberGenerator.Fill(saltBytes);
        Rfc2898DeriveBytes.Pbkdf2(password, saltBytes, keyBytes, Iterations, Algorithm);

        return Convert.ToBase64String(hashedPasswordBytes);
    }

    /// <summary>
    ///     Verifies a hash created by GetPbkdf2Hash method.
    /// </summary>
    /// <param name="hashedPassword"></param>
    /// <param name="providedPassword"></param>
    /// <returns></returns>
    public static bool IsValidPbkdf2Hash(this string hashedPassword, string providedPassword)
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
        Rfc2898DeriveBytes.Pbkdf2(providedPassword, saltBytes, actualKeyBytes, Iterations, Algorithm);

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