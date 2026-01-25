namespace DNTCommon.Web.Core;

public enum FetchResultKind
{
    Success,
    Redirected,
    Blocked,
    LoginRequired,
    Challenge,
    Failed
}
