namespace DNTCommon.Web.Core;

public class GeminiGenerateContentResponse
{
    public GeminiGenerateContentRequest? ContentRequest { set; get; }

    public bool IsBlocked { set; get; }

    public bool IsFinished { set; get; }

    public GeminiResponseContent? ResponseContent { set; get; }

    public List<GeminiPart>? ResponseParts => ResponseContent?.Candidates?.Single().Content?.Parts;
}