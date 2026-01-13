namespace DNTCommon.Web.Core;

public class ValidationException : BaseException
{
    public ValidationException(string message) : base(message) { }

    public ValidationException(string message, Exception innerException) : base(message, innerException) { }

    public ValidationException(IDictionary<string, string[]> errors) : base(
        message: "One or more validation errors occurred.")
        => Errors = errors;

    public IDictionary<string, string[]> Errors { get; } = new Dictionary<string, string[]>();
}
