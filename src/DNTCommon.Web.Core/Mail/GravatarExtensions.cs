namespace DNTCommon.Web.Core;

/// <summary>
///     Gravatar Extensions
/// </summary>
public static class GravatarExtensions
{
    /// <summary>
    ///     Returns the avatar url's of the given email address
    /// </summary>
    public static string? CalculateGravatar(this string emailAddress,
        int size = 23,
        GravatarRating rating = GravatarRating.PG)
    {
        if (string.IsNullOrWhiteSpace(emailAddress))
        {
            return null;
        }

        var hash = emailAddress.ToLowerInvariant().Trim().Md5Hash().ToLowerInvariant();

        return Invariant($"https://www.gravatar.com/avatar/{hash}.jpg?s={size}&d=identicon&r={rating}");
    }
}