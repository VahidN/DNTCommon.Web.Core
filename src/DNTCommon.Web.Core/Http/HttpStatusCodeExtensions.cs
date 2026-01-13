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

        /// <summary>
        ///     Found or Moved or Unauthorized or Forbidden or OK
        /// </summary>
        public bool IsUrlAvailable => status is HttpStatusCode.Found or HttpStatusCode.Moved or
            HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden or HttpStatusCode.OK;
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

    extension(int code)
    {
        /// <summary>
        ///     is between 200 and 300
        /// </summary>
        public bool IsSuccessHttpStatusCode => code.ToHttpStatusCode().IsSuccess;

        /// <summary>
        ///     is between 300 and 400
        /// </summary>
        public bool IsRedirectHttpStatusCode => code.ToHttpStatusCode().IsRedirect;

        /// <summary>
        ///     is between 400 and 500
        /// </summary>
        public bool IsClientErrorHttpStatusCode => code.ToHttpStatusCode().IsClientError;

        /// <summary>
        ///     is between 500 and 600
        /// </summary>
        public bool IsServerErrorHttpStatusCode => code.ToHttpStatusCode().IsServerError;

        /// <summary>
        ///     Found or Moved or Unauthorized or Forbidden or OK
        /// </summary>
        public bool IsUrlAvailableHttpStatusCode => code.ToHttpStatusCode().IsUrlAvailable;

        /// <summary>
        ///     returns (HttpStatusCode)code
        /// </summary>
        public HttpStatusCode ToHttpStatusCode() => (HttpStatusCode)code;
    }
}
