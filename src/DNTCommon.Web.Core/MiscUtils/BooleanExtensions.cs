namespace DNTCommon.Web.Core;

/// <summary>
///     Boolean Extensions
/// </summary>
public static class BooleanExtensions
{
    extension(bool)
    {
        /// <summary>
        ///     Returns a cryptographically secure bool value
        /// </summary>
        public static bool RandomBoolean
        {
            get
            {
                Span<byte> buffer = stackalloc byte[1];
                RandomNumberGenerator.Fill(buffer);

                // Use the lowest bit to determine true or false
                var randomBoolean = (buffer[index: 0] & 1) == 1;

                return randomBoolean;
            }
        }
    }
}
