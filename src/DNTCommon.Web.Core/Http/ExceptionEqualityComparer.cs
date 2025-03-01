namespace DNTCommon.Web.Core;

/// <summary>
///     Exception EqualityComparer
/// </summary>
public class ExceptionEqualityComparer : IEqualityComparer<Exception>
{
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
