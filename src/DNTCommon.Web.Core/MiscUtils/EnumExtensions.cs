namespace DNTCommon.Web.Core;

/// <summary>
///     Converts the string representation of the name or numeric value of one or more enumerated constants
/// </summary>
public static class EnumExtensions
{
    extension<T>(T _)
        where T : struct, Enum
    {
        /// <summary>
        ///     Converts the string representation of the name or numeric value of one or more enumerated constants specified by
        ///     TEnum to an equivalent enumerated object.
        /// </summary>
        public static T Parse(string value) => Enum.Parse<T>(value);

        /// <summary>
        ///     Converts the string representation of the name or numeric value of one or more enumerated constants specified by
        ///     TEnum to an equivalent enumerated object. A parameter specifies whether the operation is case-insensitive.
        /// </summary>
        public static T Parse(string value, bool ignoreCase) => Enum.Parse<T>(value, ignoreCase);

        /// <summary>
        ///     Converts the span of characters representation of the name or numeric value of one or more enumerated constants
        ///     specified by TEnum to an equivalent enumerated object.
        /// </summary>
        public static T Parse(ReadOnlySpan<char> value) => Enum.Parse<T>(value);

        /// <summary>
        ///     Converts the span of characters representation of the name or numeric value of one or more enumerated constants
        ///     specified by TEnum to an equivalent enumerated object. A parameter specifies whether the operation is
        ///     case-insensitive.
        /// </summary>
        public static T Parse(ReadOnlySpan<char> value, bool ignoreCase) => Enum.Parse<T>(value, ignoreCase);

        /// <summary>
        ///     Converts the string representation of the name or numeric value of one or more enumerated constants to an
        ///     equivalent enumerated object. The return value indicates whether the conversion succeeded.
        /// </summary>
        public static bool TryParse([NotNullWhen(returnValue: true)] string? value, out T result)
            => Enum.TryParse(value, out result);

        /// <summary>
        ///     Converts the string representation of the name or numeric value of one or more enumerated constants to an
        ///     equivalent enumerated object. A parameter specifies whether the operation is case-sensitive. The return value
        ///     indicates whether the conversion succeeded.
        /// </summary>
        public static bool TryParse([NotNullWhen(returnValue: true)] string? value, bool ignoreCase, out T result)
            => Enum.TryParse(value, ignoreCase, out result);

        /// <summary>
        ///     Converts the string representation of the name or numeric value of one or more enumerated constants to an
        ///     equivalent enumerated object.
        /// </summary>
        public static bool TryParse(ReadOnlySpan<char> value, out T result) => Enum.TryParse(value, out result);

        /// <summary>
        ///     Converts the string representation of the name or numeric value of one or more enumerated constants to an
        ///     equivalent enumerated object. A parameter specifies whether the operation is case-sensitive. The return value
        ///     indicates whether the conversion succeeded.
        /// </summary>
        public static bool TryParse(ReadOnlySpan<char> value, bool ignoreCase, out T result)
            => Enum.TryParse(value, ignoreCase, out result);

        /// <summary>
        ///     Converts the string representation of the name or numeric value of one or more enumerated constants to an
        ///     equivalent enumerated object. A parameter specifies whether the operation is case-sensitive. The return value
        ///     indicates whether the conversion succeeded.
        /// </summary>
        public static T FromValue([NotNullIfNotNull(nameof(value))] string? value,
            T defaultValue = default,
            bool ignoreCase = true)
            => value.ToEnum(defaultValue, ignoreCase);

        /// <summary>
        ///     Retrieves an array of the values of the constants in a specified enumeration type.
        /// </summary>
        /// <returns></returns>
        public static T[] GetValues() => Enum.GetValues<T>();

        /// <summary>
        ///     Retrieves an array of the names of the constants in a specified enumeration type.
        /// </summary>
        /// <returns></returns>
        public static string[] GetNames() => Enum.GetNames<T>();

        /// <summary>
        ///     Returns a random item of from a give Enum
        /// </summary>
        public static T GetRandomItem()
        {
            var values = GetValues<T>();

            return values[RandomNumberGenerator.GetInt32(values.Length)];
        }
    }
}
