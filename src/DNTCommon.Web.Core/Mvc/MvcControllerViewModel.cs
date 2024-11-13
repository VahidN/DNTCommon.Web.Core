namespace DNTCommon.Web.Core;

/// <summary>
///     MvcController ViewModel
/// </summary>
public class MvcControllerViewModel
{
    /// <summary>
    ///     Return `AreaAttribute.RouteValue`
    /// </summary>
    public string? AreaName { get; set; }

    /// <summary>
    ///     Returns the list of the Controller's Attributes.
    /// </summary>
    public IList<Attribute> ControllerAttributes { get; set; } = [];

    /// <summary>
    ///     Returns the `DisplayNameAttribute` value
    /// </summary>
    public string? ControllerDisplayName { get; set; }

    /// <summary>
    ///     It's set to `{AreaName}:{ControllerName}`
    /// </summary>
    public string ControllerId => $"{AreaName}:{ControllerName}";

    /// <summary>
    ///     Return ControllerActionDescriptor.ControllerName
    /// </summary>
    public string ControllerName { get; set; } = default!;

    /// <summary>
    ///     Returns the list of the Controller's action methods.
    /// </summary>
    public IList<MvcActionViewModel> MvcActions { get; set; } = [];

    /// <summary>
    ///     Returns `[{controllerAttributes}]{AreaName}.{ControllerName}`
    /// </summary>
    public override string ToString()
    {
        const string attribute = "Attribute";

        var controllerAttributes = string.Join(separator: ",",
            ControllerAttributes.Select(
                a => a.GetType().Name.Replace(attribute, newValue: "", StringComparison.Ordinal)));

        return $"[{controllerAttributes}]{AreaName}.{ControllerName}";
    }
}