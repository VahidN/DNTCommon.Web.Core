namespace DNTCommon.Web.Core;

public interface ISystemTimeProvider
{
    DateTime DtUtcNow { get; }

    DateTimeOffset DtoUtcNow { get; }
}
