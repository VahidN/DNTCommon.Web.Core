namespace DNTCommon.Web.Core;

/// <summary>
///     Determines the current rendering environment of a Blazor page.
///     Inspired from https://github.com/dotnet/aspnetcore/issues/49401
/// </summary>
public interface IBlazorRenderingContext
{
    /// <summary>
    ///     Determines the current rendering environment of a Blazor page.
    /// </summary>
    BlazorRenderingEnvironment GetCurrentRenderingEnvironment();
}