namespace DNTCommon.Web.Core;

/// <summary>
///     If an error is expected, don't use exceptions for flow control!
///     Only use exceptions for errors which we can't recover from (database connection errors...)
/// </summary>
public class OperationResult
{
    /// <summary>
    ///     Creates a failed operation.
    /// </summary>
    internal OperationResult()
    {
    }

    internal OperationResult(string? message, OperationStat stat)
    {
        Stat = stat;
        Message = message;
    }

    /// <summary>
    ///     Creates a succeeded operation.
    /// </summary>
    internal OperationResult(string? message)
    {
        Stat = OperationStat.Succeeded;
        Message = message;
    }

    /// <summary>
    ///     An optional result's message. It can be an error or success message.
    /// </summary>
    public string? Message { set; get; }

    /// <summary>
    ///     The operation's status
    /// </summary>
    public OperationStat Stat { set; get; }

    /// <summary>
    ///     Represents a successful operation
    /// </summary>
    public bool IsSuccess => Stat == OperationStat.Succeeded;

    /// <summary>
    ///     This will allow you to convert this object to a tuple.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="stat"></param>
    public void Deconstruct(out string? message, out OperationStat stat)
    {
        stat = Stat;
        message = Message;
    }

    /// <summary>
    ///     Creates a new operation result with a given stat
    /// </summary>
    public static implicit operator OperationResult(OperationStat stat)
        => new()
        {
            Stat = stat
        };

    /// <summary>
    ///     Returns a succeeded result.
    /// </summary>
    public static implicit operator OperationResult(string? message)
        => new()
        {
            Stat = OperationStat.Succeeded,
            Message = message
        };

    /// <summary>
    ///     Creates a new operation result
    /// </summary>
    public static implicit operator OperationResult((string? Message, OperationStat Status) operation)
        => new()
        {
            Stat = operation.Status,
            Message = operation.Message
        };

    /// <summary>
    ///     Returns a default failed result.
    /// </summary>
    public static OperationResult ToOperationResult() => new();

    /// <summary>
    ///     Creates a new succeeded operation result
    /// </summary>
    public static OperationResult Succeeded(string? message = null) => new(message, OperationStat.Succeeded);

    /// <summary>
    ///     Creates a new succeeded operation result
    /// </summary>
    public static OperationResult<TData> Succeeded<TData>(TData? result, string? message = null)
        => new(message, result);

    /// <summary>
    ///     Creates a new failed operation result
    /// </summary>
    public static OperationResult Failed(string? message = null) => new(message, OperationStat.Failed);

    /// <summary>
    ///     Creates a new failed operation result
    /// </summary>
    public static OperationResult<TData> Failed<TData>(string? message = null)
        => new(message, OperationStat.Failed, result: default);
}

/// <summary>
///     If an error is expected, don't use exceptions for flow control!
///     Only use exceptions for errors which we can't recover from (database connection errors...)
/// </summary>
public class OperationResult<TData> : OperationResult
{
    /// <summary>
    ///     Creates a failed operation.
    /// </summary>
    internal OperationResult()
    {
    }

    internal OperationResult(string? message, OperationStat stat, TData? result)
    {
        Message = message;
        Stat = stat;
        Result = result;
    }

    /// <summary>
    ///     Creates a succeeded operation.
    /// </summary>
    internal OperationResult(string? message, TData? result)
    {
        Message = message;
        Stat = OperationStat.Succeeded;
        Result = result;
    }

    /// <summary>
    ///     The result of the operation.
    /// </summary>
    public TData? Result { set; get; }

    /// <summary>
    ///     Creates a new operation result
    /// </summary>
    public static implicit operator OperationResult<TData>(OperationStat stat)
        => new()
        {
            Message = null,
            Stat = stat
        };

    /// <summary>
    ///     Returns a succeeded result.
    /// </summary>
    public static implicit operator OperationResult<TData>(TData? result)
        => new()
        {
            Message = null,
            Stat = OperationStat.Succeeded,
            Result = result
        };

    /// <summary>
    ///     Returns a succeeded result.
    /// </summary>
    public static implicit operator OperationResult<TData>((string? Message, TData? Result) operation)
        => new()
        {
            Message = operation.Message,
            Stat = OperationStat.Succeeded,
            Result = operation.Result
        };

    /// <summary>
    ///     Creates a new operation result
    /// </summary>
    public static implicit operator OperationResult<TData>((string? Message, OperationStat Status) operation)
        => new()
        {
            Message = operation.Message,
            Stat = operation.Status
        };

    /// <summary>
    ///     Creates a new operation result
    /// </summary>
    public static implicit operator OperationResult<TData>(
        (string? Message, OperationStat Status, TData? Result) operation)
        => new()
        {
            Message = operation.Message,
            Stat = operation.Status,
            Result = operation.Result
        };

    /// <summary>
    ///     This will allow you to convert this object to a tuple.
    /// </summary>
    public void Deconstruct(out string? message, out OperationStat stat, out TData? result)
    {
        stat = Stat;
        message = Message;
        result = Result;
    }

    /// <summary>
    ///     Returns a default failed result.
    /// </summary>
    public new OperationResult<TData> ToOperationResult() => new();
}