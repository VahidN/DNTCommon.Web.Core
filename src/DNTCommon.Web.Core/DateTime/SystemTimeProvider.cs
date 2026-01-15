namespace DNTCommon.Web.Core;

public class SystemTimeProvider : ISystemTimeProvider
{
    public DateTime DtUtcNow => DateTime.UtcNow;

    public DateTimeOffset DtoUtcNow => DateTimeOffset.UtcNow;
}
