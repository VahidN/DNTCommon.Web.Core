using System.Collections;

namespace DNTCommon.Web.Core;

/// <summary>
///     A Controller Dto IEqualityComparer
/// </summary>
public class MvcControllerEqualityComparer : IEqualityComparer<MvcControllerViewModel>, IEqualityComparer
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

        if (x is MvcControllerViewModel a && y is MvcControllerViewModel b)
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

        if (obj is MvcControllerViewModel x)
        {
            return GetHashCode(x);
        }

        throw new ArgumentException(message: "", nameof(obj));
    }

    /// <summary>
    ///     A Controller Dto IEqualityComparer
    /// </summary>
    public bool Equals(MvcControllerViewModel? x, MvcControllerViewModel? y)
    {
        if (ReferenceEquals(x, y))
        {
            return true;
        }

        if (x is null)
        {
            return false;
        }

        if (y is null)
        {
            return false;
        }

        if (x.GetType() != y.GetType())
        {
            return false;
        }

        return string.Equals(x.AreaName, y.AreaName, StringComparison.Ordinal) &&
               string.Equals(x.ControllerName, y.ControllerName, StringComparison.Ordinal);
    }

    /// <summary>
    ///     A Controller Dto IEqualityComparer
    /// </summary>
    public int GetHashCode(MvcControllerViewModel obj)
    {
        ArgumentNullException.ThrowIfNull(obj);

        return HashCode.Combine(obj.AreaName, obj.ControllerName);
    }
}
