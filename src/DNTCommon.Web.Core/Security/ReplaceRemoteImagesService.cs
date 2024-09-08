using HtmlAgilityPack;
using Microsoft.Extensions.Logging;

namespace DNTCommon.Web.Core;

/// <summary>
///     A helper method to download and fix remote images linked from other sites
/// </summary>
public class ReplaceRemoteImagesService(BaseHttpClient baseHttpClient, ILogger<ReplaceRemoteImagesService> logger)
    : IReplaceRemoteImagesService
{
    /// <summary>
    ///     A helper method to download and fix remote images linked from other sites
    /// </summary>
    public void FixRemoteImages(HtmlNode? node, FixRemoteImagesOptions? options)
    {
        try
        {
            if (node is null || options?.ImageUrlBuilder is null || options.OutputImageFolder is null ||
                !node.IsImageNode())
            {
                return;
            }

            AlignCenterImage(node);

            var hostUri = options.HostUri;

            if (hostUri is null)
            {
                return;
            }

            var imageSrcAttribute = node.GetSrcAttribute();
            var imageSrcValue = imageSrcAttribute?.Value?.Trim();

            if (imageSrcAttribute is null || imageSrcValue is null)
            {
                return;
            }

            if (imageSrcValue.StartsWith(value: "file:/", StringComparison.OrdinalIgnoreCase))
            {
                node.Remove();
                logger.LogWarning(message: "Removed an image tag with src: {Src}", imageSrcValue);

                return;
            }

            string? savedFileName = null;

            if (imageSrcValue.IsBase64EncodedImage())
            {
                var imageData = imageSrcValue.GetBase64EncodedImageData();
                savedFileName = SaveDataFile(imageData, options);
            }
            else if (imageSrcValue.IsRemoteUrl(hostUri))
            {
                savedFileName = DownloadImageUrlSaveFile(imageSrcValue, options);
            }

            if (!string.IsNullOrEmpty(savedFileName))
            {
                imageSrcAttribute.Value = options.ImageUrlBuilder(savedFileName);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Demystify(), message: "FixRemoteImages Error");
        }
    }

    private static void AlignCenterImage(HtmlNode node)
        => node.SetAttributeValue(name: "style", value: "display: block; margin-left: auto; margin-right: auto;");

    private string DownloadImageUrlSaveFile(string imageUrl, FixRemoteImagesOptions options)
    {
        if (string.IsNullOrEmpty(options.OutputImageFolder))
        {
            return string.Empty;
        }

        var ext = imageUrl.GetUriExtension();

        if (string.IsNullOrWhiteSpace(ext))
        {
            ext = ".jpg";
        }

        var fileName = $"{imageUrl.GetSha1Hash()}{ext}";
        var filePath = Path.Combine(options.OutputImageFolder, fileName);

        if (File.Exists(filePath))
        {
            return fileName;
        }

        var imageData = baseHttpClient.HttpClient.DownloadData(imageUrl);

        if (!imageData.IsValidImageFile(options.MaxWidth, options.MaxHeight))
        {
            imageData = imageData.ResizeImage(options.ResizeImageOptions);
            filePath = Path.ChangeExtension(filePath, extension: ".jpg");
            fileName = Path.ChangeExtension(fileName, extension: ".jpg");
        }

        if (imageData is null)
        {
            return string.Empty;
        }

        File.WriteAllBytes(filePath, imageData);

        return fileName;
    }

    private static string SaveDataFile(byte[]? imageData, FixRemoteImagesOptions options)
    {
        if (string.IsNullOrEmpty(options.OutputImageFolder) || imageData is null)
        {
            return string.Empty;
        }

        var fileName = $"{imageData.GetSha1Hash()}.jpg";
        var filePath = Path.Combine(options.OutputImageFolder, fileName);

        if (File.Exists(filePath))
        {
            return fileName;
        }

        if (!imageData.IsValidImageFile(options.MaxWidth, options.MaxHeight))
        {
            imageData = imageData.ResizeImage(options.ResizeImageOptions);
        }

        if (imageData is null)
        {
            return string.Empty;
        }

        File.WriteAllBytes(filePath, imageData);

        return fileName;
    }
}