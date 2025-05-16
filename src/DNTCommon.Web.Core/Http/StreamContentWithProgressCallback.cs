namespace DNTCommon.Web.Core;

/// <summary>
///     A custom StreamContent implementation to track and report progress during upload.
/// </summary>
public class StreamContentWithProgressCallback(Stream inputStream, Action<double> progressCallback) : HttpContent
{
    /// <summary>
    ///     Serializes the content of the stream to a target stream asynchronously
    ///     while tracking and reporting the upload progress.
    /// </summary>
    /// <param name="stream">The target stream where the content will be serialized.</param>
    /// <param name="context">An optional transport context that provides additional information about the stream operation.</param>
    /// <returns>A task representing the asynchronous operation of writing the content to the target stream.</returns>
    protected override async Task SerializeToStreamAsync(Stream stream, TransportContext? context)
    {
        if (stream is null)
        {
            return;
        }

        var buffer = new byte[81920]; // 80 KB buffer size
        var totalBytes = inputStream.Length;
        var uploadedBytes = 0L;

        while (true)
        {
            var bytesRead = await inputStream.ReadAsync(buffer);

            if (bytesRead == 0)
            {
                break;
            }

            await stream.WriteAsync(buffer.AsMemory(start: 0, bytesRead));

            uploadedBytes += bytesRead;

            // Report progress
            var progress = (double)uploadedBytes / totalBytes * 100;

            progressCallback?.Invoke(progress);
        }
    }

    /// <summary>
    ///     Attempts to compute the length of the stream content.
    /// </summary>
    /// <param name="length">When this method returns, contains the computed length of the stream if it exists; otherwise, 0.</param>
    /// <returns>true if the length of the stream can be determined; otherwise, false.</returns>
    protected override bool TryComputeLength(out long length)
    {
        length = inputStream.Length;

        return true;
    }
}
