using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace DNTCommon.Web.Core;

/// <summary>
///     Do not convert an empty string value or one containing only whitespace characters to null when representing a model
///     as text
/// </summary>
public class EmptyStringEnabledDisplayMetadataProvider : IDisplayMetadataProvider
{
    /// <inheritdoc />
    public void CreateDisplayMetadata(DisplayMetadataProviderContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (context.Key.ModelType == typeof(string))
        {
            context.DisplayMetadata.ConvertEmptyStringToNull = false;
        }
    }
}