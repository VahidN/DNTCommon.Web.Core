namespace DNTCommon.Web.Core;

public class NotFoundException : BaseException
{
    public NotFoundException(string message) : base(message) { }

    public NotFoundException(string name, object key) : base($"Entity '{name}' with key '{key}' was not found.") { }
}
