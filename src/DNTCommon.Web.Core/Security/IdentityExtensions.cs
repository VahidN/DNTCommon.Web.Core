using System;
using System.Globalization;
using System.Security.Claims;
using System.Security.Principal;

namespace DNTCommon.Web.Core
{
    /// <summary>
    /// ClaimsIdentity extensions
    /// </summary>
    public static class IdentityExtensions
    {
        /// <summary>
        /// Finds the first claimType's value of the given ClaimsIdentity.
        /// </summary>
        public static string? FindFirstValue(this ClaimsIdentity identity, string claimType)
        {
            return identity?.FindFirst(claimType)?.Value;
        }

        /// <summary>
        /// Finds the first claimType's value of the given IIdentity.
        /// </summary>
        public static string? GetUserClaimValue(this IIdentity identity, string claimType)
        {
            var identity1 = identity as ClaimsIdentity;
            return identity1?.FindFirstValue(claimType);
        }

        /// <summary>
        /// Extracts the `ClaimTypes.GivenName`'s value of the given IIdentity.
        /// </summary>
        public static string? GetUserFirstName(this IIdentity identity)
        {
            return identity?.GetUserClaimValue(ClaimTypes.GivenName);
        }

        /// <summary>
        /// Extracts the `ClaimTypes.NameIdentifier`'s value of the given IIdentity.
        /// </summary>
        public static T? GetUserId<T>(this IIdentity identity) where T : IConvertible
        {
            var firstValue = identity?.GetUserClaimValue(ClaimTypes.NameIdentifier);
            return firstValue != null
                ? (T)Convert.ChangeType(firstValue, typeof(T), CultureInfo.InvariantCulture)
                : default;
        }

        /// <summary>
        /// Extracts the `ClaimTypes.NameIdentifier`'s value of the given IIdentity.
        /// </summary>
        public static string? GetUserId(this IIdentity identity)
        {
            return identity?.GetUserClaimValue(ClaimTypes.NameIdentifier);
        }

        /// <summary>
        /// Extracts the `ClaimTypes.NameIdentifier`'s value of the given IIdentity.
        /// </summary>
        public static int? GetUserId(this ClaimsPrincipal user)
        {
            var userIdValue = user?.Identity?.GetUserId();
            if (string.IsNullOrWhiteSpace(userIdValue))
            {
                return null;
            }

            if (int.TryParse(userIdValue, out var userId))
            {
                return userId;
            }
            return null;
        }

        /// <summary>
        /// Extracts the `ClaimTypes.Surname`'s value of the given IIdentity.
        /// </summary>
        public static string? GetUserLastName(this IIdentity identity)
        {
            return identity?.GetUserClaimValue(ClaimTypes.Surname);
        }

        /// <summary>
        /// Extracts the `ClaimTypes.GivenName` + `ClaimTypes.Surname`'s value of the given IIdentity.
        /// </summary>
        public static string GetUserFullName(this IIdentity identity)
        {
            return $"{GetUserFirstName(identity)} {GetUserLastName(identity)}";
        }

        /// <summary>
        /// Return the GetUserFullName, otherwise the `ClaimTypes.Name`'s value.
        /// </summary>
        public static string? GetUserDisplayName(this IIdentity identity)
        {
            var fullName = GetUserFullName(identity);
            return string.IsNullOrWhiteSpace(fullName) ? GetUserName(identity) : fullName;
        }

        /// <summary>
        /// Extracts the `ClaimTypes.Name`'s value of the given IIdentity.
        /// </summary>
        public static string? GetUserName(this IIdentity identity)
        {
            return identity?.GetUserClaimValue(ClaimTypes.Name);
        }
    }
}