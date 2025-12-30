namespace DNTCommon.Web.Core;

public class GeminiResponseResult<T>
{
    public Uri? FullApiUri { set; get; }

    public bool? IsSuccessfulResponse { set; get; }

    public GeminiErrorResponse? ErrorResponse { set; get; }

    public string? ResponseBody { set; get; }

    public T? Result { set; get; }
}