namespace DNTCommon.Web.Core;

public record BaleApiResponseStatus(bool Success, HttpStatusCode StatusCode, string ResponseContent);
