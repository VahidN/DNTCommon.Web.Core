using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;

namespace DNTCommon.Web.Core;

/// <summary>
///     AuthenticationState Extensions
/// </summary>
public static class AuthenticationStateExtensions
{
    /// <summary>
    ///     Is current user authenticated
    /// </summary>
    public static bool IsAuthenticated(this AuthenticationState? authState)
    {
        var user = authState?.User;

        return IsAuthenticated(user);
    }

    /// <summary>
    ///     Is current user authenticated
    /// </summary>
    public static bool IsAuthenticated(this HttpContext? context)
    {
        var user = context?.User;

        return IsAuthenticated(user);
    }

    /// <summary>
    ///     Is current user authenticated
    /// </summary>
    public static bool IsAuthenticated(this IPrincipal? user)
        => user?.Identity is not null && user.Identity.IsAuthenticated;

    /// <summary>
    ///     Is current user is in the specified comma-separated roles?
    /// </summary>
    public static bool IsUserInRole(this AuthenticationState? authState, string? allowedRoles)
    {
        var user = authState?.User;

        return IsUserInRole(user, allowedRoles);
    }

    /// <summary>
    ///     Is current user is in the specified comma-separated roles?
    /// </summary>
    public static bool IsUserInRole(this HttpContext? context, string? allowedRoles)
    {
        var user = context?.User;

        return IsUserInRole(user, allowedRoles);
    }

    /// <summary>
    ///     Is current user is in the specified comma-separated roles?
    /// </summary>
    public static bool IsUserInRole(this IPrincipal? user, string? allowedRoles)
    {
        if (user?.Identity is null)
        {
            return false;
        }

        if (!user.IsAuthenticated())
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(allowedRoles))
        {
            return false;
        }

        var roles = allowedRoles.Split(separator: ',', StringSplitOptions.RemoveEmptyEntries);

        return roles.Any(user.IsInRole);
    }

    /// <summary>
    ///     Does current user have the specified comma-separated user claims?
    /// </summary>
    public static bool HasUserClaims(this AuthenticationState? authState, params Claim[]? allowedClaims)
    {
        var user = authState?.User;

        return HasUserClaims(user, allowedClaims);
    }

    /// <summary>
    ///     Does current user have the specified comma-separated user claims?
    /// </summary>
    public static bool HasUserClaims(this HttpContext? context, params Claim[]? allowedClaims)
    {
        var user = context?.User;

        return HasUserClaims(user, allowedClaims);
    }

    /// <summary>
    ///     Does current user have the specified comma-separated user claims?
    /// </summary>
    public static bool HasUserClaims(this ClaimsPrincipal? user, params Claim[]? allowedClaims)
    {
        if (user?.Identity is null)
        {
            return false;
        }

        if (!user.IsAuthenticated())
        {
            return false;
        }

        if (allowedClaims is null)
        {
            return false;
        }

        return allowedClaims.Any(claim => user.HasClaim(claim.Type, claim.Value));
    }

    /// <summary>
    ///     Does the current user have access to this menu item?
    /// </summary>
    public static bool HasUserAccess(this ClaimsPrincipal? user,
        bool allowAnonymous,
        IReadOnlyList<Claim>? allowedClaims,
        string? allowedRoles)
    {
        if (allowAnonymous)
        {
            return true;
        }

        if (allowedRoles is not null && allowedClaims is null)
        {
            return user.IsUserInRole(allowedRoles);
        }

        if (allowedRoles is null && allowedClaims is not null)
        {
            return user.HasUserClaims([.. allowedClaims]);
        }

        if (allowedRoles is not null && allowedClaims is not null)
        {
            return user.HasUserClaims([.. allowedClaims]) && user.IsUserInRole(allowedRoles);
        }

        return user.IsAuthenticated();
    }

    /// <summary>
    ///     Does the current user have access to this menu item?
    /// </summary>
    public static bool HasUserAccess(this AuthenticationState? authState,
        bool allowAnonymous,
        IReadOnlyList<Claim>? allowedClaims,
        string? allowedRoles)
    {
        var user = authState?.User;

        return HasUserAccess(user, allowAnonymous, allowedClaims, allowedRoles);
    }

    /// <summary>
    ///     Does the current user have access to this menu item?
    /// </summary>
    public static bool HasUserAccess(this HttpContext? context,
        bool allowAnonymous,
        IReadOnlyList<Claim>? allowedClaims,
        string? allowedRoles)
    {
        var user = context?.User;

        return HasUserAccess(user, allowAnonymous, allowedClaims, allowedRoles);
    }

    /// <summary>
    ///     Returns the first value of the given claim type.
    /// </summary>
    public static string? GetFirstUserClaimValue(this AuthenticationState? authState, string claimType)
    {
        var user = authState?.User;

        return GetFirstUserClaimValue(user, claimType);
    }

    /// <summary>
    ///     Returns the first value of the given claim type.
    /// </summary>
    public static string? GetFirstUserClaimValue(this HttpContext? context, string claimType)
    {
        var user = context?.User;

        return GetFirstUserClaimValue(user, claimType);
    }

    /// <summary>
    ///     Returns the first value of the given claim type.
    /// </summary>
    public static string? GetFirstUserClaimValue(this ClaimsPrincipal? user, string claimType)
        => user?.Claims.FirstOrDefault(c => string.Equals(c.Type, claimType, StringComparison.Ordinal))?.Value;

    /// <summary>
    ///     Returns the list of the values of the given claim type.
    /// </summary>
    public static IList<string> GetUserClaimValues(this AuthenticationState? authState, string claimType)
    {
        var user = authState?.User;

        return GetUserClaimValues(user, claimType);
    }

    /// <summary>
    ///     Returns the list of the values of the given claim type.
    /// </summary>
    public static IList<string> GetUserClaimValues(this HttpContext? context, string claimType)
    {
        var user = context?.User;

        return GetUserClaimValues(user, claimType);
    }

    /// <summary>
    ///     Returns the list of the values of the given claim type.
    /// </summary>
    public static IList<string> GetUserClaimValues(this ClaimsPrincipal? user, string claimType)
    {
        if (user is null)
        {
            return [];
        }

        return user.Claims.Where(c => string.Equals(c.Type, claimType, StringComparison.Ordinal))
            .Select(c => c.Value)
            .ToList();
    }

    /// <summary>
    ///     Returns a customized version of `User.Identity.Name` using a provided claim first.
    /// </summary>
    public static string GetDisplayName(this AuthenticationState? authState, string claimType, string defaultValue)
    {
        var user = authState?.User;

        return GetDisplayName(user, claimType, defaultValue);
    }

    /// <summary>
    ///     Returns a customized version of `User.Identity.Name` using a provided claim first.
    /// </summary>
    public static string GetDisplayName(this HttpContext? context, string claimType, string defaultValue)
    {
        var user = context?.User;

        return GetDisplayName(user, claimType, defaultValue);
    }

    /// <summary>
    ///     Returns a customized version of `User.Identity.Name` using a provided claim first.
    /// </summary>
    public static string GetDisplayName(this ClaimsPrincipal? user, string claimType, string defaultValue)
    {
        if (user?.Identity?.IsAuthenticated != true)
        {
            return defaultValue;
        }

        var displayName = user.GetFirstUserClaimValue(claimType);

        if (!string.IsNullOrWhiteSpace(displayName))
        {
            return displayName;
        }

        displayName = user.Identity.Name;

        return !string.IsNullOrWhiteSpace(displayName) ? displayName : defaultValue;
    }
}