namespace DNTCommon.Web.Core;

internal sealed class MegaLogoutRequest : MegaRequest
{
    public MegaLogoutRequest() => Action = "sml";
}