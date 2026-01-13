namespace DNTCommon.Web.Core;

public static class HttpStatusCodeExtensions
{
    extension(HttpStatusCode status)
    {
        /// <summary>
        ///     is between 200 and 300
        /// </summary>
        public bool IsSuccess => (int)status is >= 200 and < 300;

        /// <summary>
        ///     is between 300 and 400
        /// </summary>
        public bool IsRedirect => (int)status is >= 300 and < 400;

        /// <summary>
        ///     is between 400 and 500
        /// </summary>
        public bool IsClientError => (int)status is >= 400 and < 500;

        /// <summary>
        ///     is between 500 and 600
        /// </summary>
        public bool IsServerError => (int)status is >= 500 and < 600;
    }

    extension(HttpStatusCode)
    {
        /// <summary>
        ///     returns (HttpStatusCode)code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static HttpStatusCode FromInt(int code) => (HttpStatusCode)code;
    }
}
