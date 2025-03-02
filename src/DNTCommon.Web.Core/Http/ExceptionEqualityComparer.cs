using System.Collections;

namespace DNTCommon.Web.Core;

/// <summary>
///     Exception EqualityComparer
/// </summary>
public class ExceptionEqualityComparer : IEqualityComparer<Exception>, IEqualityComparer
{
    public new bool Equals(object? x, object? y)
    {
        if (x == y)
        {
            return true;
        }

        if (x is null || y is null)
        {
            return false;
        }

        if (x is Exception a && y is Exception b)
        {
            return Equals(a, b);
        }

        throw new ArgumentException(message: "", nameof(x));
    }

    public int GetHashCode(object? obj)
    {
        if (obj is null)
        {
            return 0;
        }

        if (obj is Exception x)
        {
            return GetHashCode(x);
        }

        throw new ArgumentException(message: "", nameof(obj));
    }

    /// <summary>
    ///     Checks if two exceptions are equal.
    /// </summary>
    public bool Equals(Exception? x, Exception? y)
    {
        if (y is null && x is null)
        {
            return true;
        }

        if (x is null || y is null)
        {
            return false;
        }

        return x.GetType().Name.Equals(y.GetType().Name, StringComparison.Ordinal) &&
               x.Message.Equals(y.Message, StringComparison.Ordinal);
    }

    /// <summary>
    ///     Gets the exception's hash.
    /// </summary>
    public int GetHashCode(Exception obj)
    {
        ArgumentNullException.ThrowIfNull(obj);

        return (obj.GetType().Name + obj.Message).GetHashCode(StringComparison.Ordinal);
    }
}
