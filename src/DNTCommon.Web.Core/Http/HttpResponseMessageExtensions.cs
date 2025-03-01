namespace DNTCommon.Web.Core;

/// <summary>
///     HttpResponseMessage Extensions
/// </summary>
public static class HttpResponseMessageExtensions
{
    /// <summary>
    ///     Includes the response's body in the final error message.
    /// </summary>
    public static async Task EnsureSuccessStatusCodeAsync(this HttpResponseMessage response)
    {
        ArgumentNullException.ThrowIfNull(response);

        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var content = $"StatusCode: {response.StatusCode}, {await response.Content.ReadAsStringAsync()}";
        response.Content?.Dispose();

        throw new SimpleHttpResponseException(response.StatusCode, content);
    }

    /// <summary>
    ///     Includes the response's body in the final error message.
    /// </summary>
    public static void EnsureSuccessStatusCode(this HttpResponseMessage response)
    {
        ArgumentNullException.ThrowIfNull(response);

        if (response.IsSuccessStatusCode)
        {
            return;
        }

        using var reader = new StreamReader(response.Content.ReadAsStream());
        var responseContent = reader.ReadToEnd();

        var content = $"StatusCode: {response.StatusCode}, {responseContent}";
        response.Content?.Dispose();

        throw new SimpleHttpResponseException(response.StatusCode, content);
    }
}
