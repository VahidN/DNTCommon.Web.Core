namespace DNTCommon.Web.Core
{
    /// <summary>
    /// ApiError's structure
    /// </summary>
    public class ApiErrorDto
    {
        /// <summary>
        /// Response's status code
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// Error message
        /// </summary>
        public string Message { get; set; } = default!;
    }
}