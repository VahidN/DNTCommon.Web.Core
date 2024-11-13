namespace DNTCommon.Web.Core;

/// <summary>
///     A Controller Dto IEqualityComparer
/// </summary>
public class MvcControllerEqualityComparer : IEqualityComparer<MvcControllerViewModel>
{
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
        if (obj == null)
        {
            throw new ArgumentNullException(nameof(obj));
        }

        return HashCode.Combine(obj.AreaName, obj.ControllerName);
    }
}