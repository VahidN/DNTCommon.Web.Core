namespace DNTCommon.Web.Core;

public static class GuidExtensions
{
    extension(Guid)
    {
        /// <summary>
        ///     Returns a cryptographically secure random data which is RFC 4122 compliance.
        /// </summary>
        public static Guid CryptographicallySecureGuid
        {
            get
            {
                var guidBytes = RandomNumberGenerator.GetBytes(count: 16);

                // Apply RFC 4122 compliance (version 4 UUID)
                // Set version bits (4 bits starting at bit 48)
                guidBytes[7] = (byte)((guidBytes[7] & 0x0F) | 0x40);

                // Set variant bits (2 bits starting at bit 64)
                guidBytes[8] = (byte)((guidBytes[8] & 0x3F) | 0x80);

                return new Guid(guidBytes);
            }
        }

        /// <summary>
        ///     Returns a cryptographically secure Guid which is RFC 4122 compliance.
        ///     Its lenght is 32 chars.
        /// </summary>
        public static string CryptographicallySecureApiToken
            => Guid.CryptographicallySecureGuid.ToString(format: "N").ToUpperInvariant();

        /// <summary>
        ///     Combines two secure GUIDs for extra entropy.
        ///     Its lenght is 64 chars.
        /// </summary>
        public static string CryptographicallySecureSessionToken => string.Format(CultureInfo.InvariantCulture,
            format: "{0}{0}", Guid.CryptographicallySecureApiToken);

#if !NET_6
        /// <summary>
        ///     Tries to parse a string into a value.
        /// </summary>
        public static bool IsValidGuid([NotNullWhen(returnValue: true)] string? value)
            => !value.IsEmpty() && Guid.TryParse(value, CultureInfo.InvariantCulture, out _);

        /// <summary>
        ///     Converts the string representation of the input value to an
        ///     equivalent Guid object.
        /// </summary>
        public static Guid? FromValue([NotNullIfNotNull(nameof(value))] string? value, Guid? defaultValue = null)
        {
            if (value.IsEmpty())
            {
                return defaultValue;
            }

            return Guid.TryParse(value, CultureInfo.InvariantCulture, out var result) ? result : defaultValue;
        }
#endif
    }

    extension(Guid guid)
    {
        public bool IsEmpty => guid == Guid.Empty;
    }

    extension(Guid? guid)
    {
        public bool IsEmpty => guid.HasValue && guid.Value == Guid.Empty;

        public bool IsNullOrEmpty => !guid.HasValue || guid.Value == Guid.Empty;

        public bool IsNotNullOrEmpty => guid.HasValue && guid.Value != Guid.Empty;
    }
}
