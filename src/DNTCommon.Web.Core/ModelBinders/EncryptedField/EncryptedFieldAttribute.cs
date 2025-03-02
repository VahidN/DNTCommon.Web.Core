namespace DNTCommon.Web.Core;

/// <summary>
///     Use this attribute to mark the string properties of your Models which should be
///     encrypted automatically using the `EncryptedFieldResultFilter` and
///     decrypted using the `EncryptedFieldModelBinderProvider`.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class EncryptedFieldAttribute : Attribute;
