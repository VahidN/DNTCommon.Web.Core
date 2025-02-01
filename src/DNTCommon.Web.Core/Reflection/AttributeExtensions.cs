using System.ComponentModel;

namespace DNTCommon.Web.Core;

/// <summary>
///     Attribute Extensions
/// </summary>
public static class AttributeExtensions
{
    /// <summary>
    ///     Gets the display text for an enum value.
    ///     Uses the DisplayAttribute if set on the enum member, so this support localization.
    /// </summary>
    public static string GetDisplayName<TEnum>(this TEnum value)
        where TEnum : Enum
    {
        if (value is null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        var name = value.ToString();

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentNullException(nameof(value));
        }

        var member = value.GetType().GetMember(name)[0];
        var displayAttribute = member.GetCustomAttribute<DisplayAttribute>();

        return displayAttribute?.GetName() ?? name;
    }

    /// <summary>
    ///     Determines if the type has the specified attribute.
    /// </summary>
    public static bool HasAttribute<TAttribute>([NotNullWhen(returnValue: true)] this Type? type, bool inherited = true)
        where TAttribute : Attribute
        => type?.GetTypeInfo().GetCustomAttributes(inherited).OfType<TAttribute>().Any() == true;

    /// <summary>
    ///     Returns all types which contain the specified attributes
    /// </summary>
    public static IEnumerable<Type> GetTypesWithAttributes(this Assembly assembly,
        bool inherited = true,
        params ICollection<Type>? attributes)
        => new List<Assembly>
        {
            assembly
        }.GetTypesWithAttributes(inherited, attributes);

    /// <summary>
    ///     Returns all types which contain the specified attributes
    /// </summary>
    public static IEnumerable<Type> GetTypesWithAttributes(this ICollection<Assembly>? assemblies,
        bool inherited = true,
        params ICollection<Type>? attributes)
    {
        if (assemblies is null || attributes is null)
        {
            yield break;
        }

        foreach (var type in assemblies.GetAllConcreteTypes())
        {
            var typeAttributes = type.GetTypeInfo().GetCustomAttributes(inherited);

            foreach (var attribute in attributes)
            {
                if (typeAttributes.Any(attr => attr.GetType() == attribute))
                {
                    yield return type;
                }
            }
        }
    }

    /// <summary>
    ///     Gets the specified attribute from the PropertyDescriptor.
    /// </summary>
    public static T? GetAttribute<T>(this PropertyDescriptor? prop)
        where T : Attribute
    {
        if (prop is null)
        {
            return null;
        }

        foreach (Attribute att in prop.Attributes)
        {
            if (att is T tAtt)
            {
                return tAtt;
            }
        }

        return null;
    }

    /// <summary>
    ///     Gets the specified attribute from the PropertyDescriptor.
    /// </summary>
    public static T? GetAttribute<T>(this PropertyInfo? prop, bool inherit)
        where T : Attribute
        => prop?.GetCustomAttributes(typeof(T), inherit).GetAttributeOfType<T>();

    /// <summary>
    ///     Gets the specified attribute from the type.
    /// </summary>
    public static T? GetAttribute<T>(this Type? type, bool inherit)
        where T : Attribute
        => type?.GetCustomAttributes(typeof(T), inherit).GetAttributeOfType<T>();

    /// <summary>
    ///     Gets the specified attribute for the assembly.
    /// </summary>
    public static T? GetAttribute<T>(this Assembly? asm)
        where T : Attribute
        => asm?.GetCustomAttributes(typeof(T), inherit: false).GetAttributeOfType<T>();

    /// <summary>
    ///     Gets the specified attribute from the PropertyDescriptor.
    /// </summary>
    public static T? GetAttribute<T>(this object? obj, bool inherit)
        where T : Attribute
    {
        if (obj == null)
        {
            return null;
        }

        var type = obj.GetType();

        if (type.IsDerivedFrom<PropertyDescriptor>())
        {
            return GetAttribute<T>((PropertyDescriptor)obj);
        }

        if (type.IsDerivedFrom<PropertyInfo>())
        {
            return GetAttribute<T>((PropertyInfo)obj, inherit);
        }

        if (type.IsDerivedFrom<Assembly>())
        {
            return (obj as Assembly)?.GetCustomAttributes(typeof(T), inherit: false).GetAttributeOfType<T>();
        }

        if (type.IsDerivedFrom<Type>())
        {
            return GetAttribute<T>((Type)obj, inherit);
        }

        return GetAttribute<T>(type, inherit);
    }

    /// <summary>
    ///     Gets the specified attribute from the Enum.
    /// </summary>
    public static T? GetAttribute<T>(this Enum? val)
        where T : Attribute
        => val?.GetType()
            .GetField(val.ToString())
            ?.GetCustomAttributes(typeof(T), inherit: false)
            .GetAttributeOfType<T>();

    private static T? GetAttributeOfType<T>(this object[]? attributes)
        where T : Attribute
        => attributes is null || attributes.Length == 0 ? null : (T)attributes[0];

    /// <summary>
    ///     Gets the value from the DescriptionAttribute for the given enumeration value.
    /// </summary>
    public static string? GetDescription(this Enum? e) => GetAttribute<DescriptionAttribute>(e)?.Description;

    /// <summary>
    ///     Gets the specified attributes from the PropertyDescriptor.
    /// </summary>
    public static IList<T>? GetAttributes<T>(this PropertyDescriptor? prop)
        where T : Attribute
    {
        if (prop is null)
        {
            return null;
        }

        var attributes = new List<T>();

        foreach (Attribute att in prop.Attributes)
        {
            if (att is T tAtt)
            {
                attributes.Add(tAtt);
            }
        }

        return attributes;
    }

    /// <summary>
    ///     returns an array of all custom attributes applied to this member.
    /// </summary>
    public static IList<Attribute> GetMethodAttributes(this MethodInfo mi, bool inherited = true)
    {
        ArgumentNullException.ThrowIfNull(mi);

        return mi.GetCustomAttributes(inherited).Cast<Attribute>().ToList();
    }

    /// <summary>
    ///     returns an array of all custom attributes applied to this member.
    /// </summary>
    public static IList<Attribute> GetParameterAttributes(this ParameterInfo pi, bool inherited = true)
    {
        ArgumentNullException.ThrowIfNull(pi);

        return pi.GetCustomAttributes(inherited).Cast<Attribute>().ToList();
    }

    /// <summary>
    ///     returns an array of all custom attributes applied to this member.
    /// </summary>
    public static IList<Attribute> GetPropertyAttributes(this PropertyInfo pi, bool inherited = true)
    {
        ArgumentNullException.ThrowIfNull(pi);

        return pi.GetCustomAttributes(inherited).Cast<Attribute>().ToList();
    }

    /// <summary>
    ///     returns an array of all custom attributes applied to this member.
    /// </summary>
    public static IList<Attribute> GetTypeAttributes(this Type type, bool inherited = true)
    {
        ArgumentNullException.ThrowIfNull(type);

        return type.GetCustomAttributes(inherited).Cast<Attribute>().ToList();
    }

    /// <summary>
    ///     returns an array of all custom attributes applied to this member.
    /// </summary>
    public static IList<Attribute> GetAssemblyAttributes(this Assembly assembly, bool inherited = true)
    {
        ArgumentNullException.ThrowIfNull(assembly);

        return assembly.GetCustomAttributes(inherited).Cast<Attribute>().ToList();
    }
}