namespace DNTCommon.Web.Core;

internal static class MegaErrorCodeHelper
{
    public static MegaErrorCode GetKnownOrUnknown(int code)
    {
        var value = (MegaErrorCode)code;

        return Enum.IsDefined(value) ? value : MegaErrorCode.Unknown;
    }

    public static string GetDisplayName(int code)
    {
        var value = (MegaErrorCode)code;

        return Enum.IsDefined(value) ? value.ToString() : nameof(MegaErrorCode.Unknown);
    }
}
