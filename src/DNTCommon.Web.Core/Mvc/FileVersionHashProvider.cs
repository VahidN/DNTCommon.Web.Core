using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;

namespace DNTCommon.Web.Core;

/// <summary>
///     This utility class will help you to add your `asp-append-version` like query string value to the .css or .js files.
/// </summary>
public static class FileVersionHashProvider
{
    private static readonly string ProcessExecutableModuleVersionId =
        Assembly.GetEntryAssembly()!.ManifestModule.ModuleVersionId.ToString("N");

    /// <summary>
    ///     Adds a version hash to a specified file.
    ///     If you can't use `asp-append-version` directlty in Blazor apps, use this method instead.
    /// </summary>
    /// <param name="httpContext">Encapsulates all HTTP-specific information about an individual HTTP request</param>
    /// <param name="filePath">The path of the .js or .css file to which version should be added</param>
    /// <param name="defaultHash">
    ///     If it's not possible to calculate the hash of the filePath, this value will be used.
    ///     If you don't specify it, Assembly.GetEntryAssembly()!.ManifestModule.ModuleVersionId will be used for it.
    /// </param>
    /// <returns></returns>
    public static string GetFileVersionedPath(this HttpContext httpContext, string filePath, string? defaultHash = null)
    {
        ArgumentNullException.ThrowIfNull(httpContext);

        var fileVersionedPath = httpContext.RequestServices.GetRequiredService<IFileVersionProvider>()
            .AddFileVersionToPath(httpContext.Request.PathBase, filePath);

        return IsEmbeddedOrNotFound(fileVersionedPath, filePath)
            ? QueryHelpers.AddQueryString(filePath, new Dictionary<string, string?>(StringComparer.Ordinal)
            {
                {
                    "v", defaultHash ?? ProcessExecutableModuleVersionId
                }
            })
            : fileVersionedPath;
    }

    private static bool IsEmbeddedOrNotFound(string fileVersionedPath, string filePath)
        => string.Equals(fileVersionedPath, filePath, StringComparison.Ordinal);
}