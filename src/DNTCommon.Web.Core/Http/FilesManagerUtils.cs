using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Forms;

namespace DNTCommon.Web.Core;

/// <summary>
///     Provides helpers methods to work with files and forms.
/// </summary>
public static class FilesManagerUtils
{
    /// <summary>
    ///     Posts a model with all of its attached files to the server.
    /// </summary>
    /// <param name="httpClient">Sends HTTP requests and receives HTTP responses</param>
    /// <param name="requestUri">The Web-API action method's URI</param>
    /// <param name="model">The model's instance to be posted to the server</param>
    /// <param name="modelParameterName">The parameter name of the action method which accepts the model's object</param>
    /// <param name="browserFiles">The selected files by the user</param>
    /// <param name="fileParameterName">The parameter name of the action method which accepts the list of the posted files</param>
    /// <param name="maxAllowedSize">The maximum number of bytes that can be supplied by the Stream of the selected files</param>
    /// <param name="cancellationToken">
    ///     A cancellation token that can be used by other objects or threads to receive notice of
    ///     cancellation
    /// </param>
    public static async Task<HttpResponseMessage> PostModelWithFilesAsync(this HttpClient httpClient,
        Uri requestUri,
        object model,
        string modelParameterName,
        ICollection<IBrowserFile>? browserFiles,
        string? fileParameterName,
        long maxAllowedSize = 512000 * 1000,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(httpClient);

        var disposableStreamContents = new List<IDisposable>();
        var disposableStreams = new List<IDisposable>();

        try
        {
            using var requestContent = new MultipartFormDataContent();
            requestContent.Headers.ContentDisposition = new ContentDispositionHeaderValue(dispositionType: "form-data");

            if (browserFiles?.Count > 0 && !string.IsNullOrWhiteSpace(fileParameterName))
            {
                foreach (var file in browserFiles)
                {
                    var stream = file.OpenReadStream(maxAllowedSize, cancellationToken);

                    if (stream is null)
                    {
                        continue;
                    }

                    var streamContent = new StreamContent(stream, (int)file.Size);
                    disposableStreamContents.Add(streamContent);
                    disposableStreams.Add(stream);
                    requestContent.Add(streamContent, fileParameterName, file.Name);
                }
            }

            using var stringContent = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8,
                mediaType: "application/json");

            requestContent.Add(stringContent, modelParameterName);

            return await httpClient.PostAsync(requestUri, requestContent, cancellationToken);
        }
        finally
        {
            foreach (var disposable in disposableStreamContents)
            {
                disposable?.Dispose();
            }

            foreach (var disposable in disposableStreams)
            {
                disposable?.Dispose();
            }
        }
    }

    /// <summary>
    ///     Posts a model with all of its attached files to the server.
    /// </summary>
    /// <param name="httpClient">Sends HTTP requests and receives HTTP responses</param>
    /// <param name="requestUri">The Web-API action method's URI</param>
    /// <param name="model">The model's instance to be posted to the server</param>
    /// <param name="modelParameterName">The parameter name of the action method which accepts the model's object</param>
    /// <param name="files">The selected files by the user</param>
    /// <param name="fileParameterName">The parameter name of the action method which accepts the list of the posted files</param>
    /// <param name="cancellationToken">
    ///     A cancellation token that can be used by other objects or threads to receive notice of
    ///     cancellation
    /// </param>
    public static async Task<HttpResponseMessage> PostModelWithFilesAsync(this HttpClient httpClient,
        Uri requestUri,
        object model,
        string modelParameterName,
        ICollection<(byte[] Content, string FileName)>? files,
        string? fileParameterName,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(httpClient);

        var disposableStreamContents = new List<IDisposable>();

        try
        {
            using var requestContent = new MultipartFormDataContent();
            requestContent.Headers.ContentDisposition = new ContentDispositionHeaderValue(dispositionType: "form-data");

            if (files?.Count > 0 && !string.IsNullOrWhiteSpace(fileParameterName))
            {
                foreach (var file in files)
                {
                    var byteArrayContent = new ByteArrayContent(file.Content);
                    disposableStreamContents.Add(byteArrayContent);
                    requestContent.Add(byteArrayContent, fileParameterName, file.FileName);
                }
            }

            using var stringContent = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8,
                mediaType: "application/json");

            requestContent.Add(stringContent, modelParameterName);

            return await httpClient.PostAsync(requestUri, requestContent, cancellationToken);
        }
        finally
        {
            foreach (var disposable in disposableStreamContents)
            {
                disposable?.Dispose();
            }
        }
    }
}