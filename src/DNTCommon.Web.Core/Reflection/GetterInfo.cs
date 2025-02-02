namespace DNTCommon.Web.Core;

/// <summary>
///     Getter method's info.
/// </summary>
public class GetterInfo
{
    /// <summary>
    ///     Property/Field's Getter method.
    /// </summary>
    public Func<object, object> GetterFunc { set; get; } = null!;

    /// <summary>
    ///     Obtains information about the attributes of a member and provides access.
    /// </summary>
    public MemberInfo MemberInfo { set; get; } = null!;

    /// <summary>
    ///     Property/Field's name.
    /// </summary>
    public string Name { set; get; } = null!;

    /// <summary>
    ///     Property/Field's Type.
    /// </summary>
    public Type PropertyType { set; get; } = null!;
}