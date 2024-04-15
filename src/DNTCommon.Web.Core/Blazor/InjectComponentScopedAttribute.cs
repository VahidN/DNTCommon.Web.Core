namespace DNTCommon.Web.Core;

/// <summary>
///     This marked service will be provided using the `OwningComponentBase` and its lifetime is limited to the lifetime of
///     the current component and won't be shared across different ones.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class InjectComponentScopedAttribute : Attribute
{
}