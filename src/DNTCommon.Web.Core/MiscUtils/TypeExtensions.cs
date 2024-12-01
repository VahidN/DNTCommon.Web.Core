using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DNTCommon.Web.Core;

/// <summary>
///     Provides extension methods for Type.
/// </summary>
public static class TypeExtensions
{
	/// <summary>
	///     Returns whether the given value object is valid for this type.
	/// </summary>
	public static bool CanCovertTo([NotNullWhen(returnValue: true)] this string? value, Type type)
        => value is not null && TypeDescriptor.GetConverter(type).IsValid(value);

	/// <summary>
	///     Returns whether the given value object is valid for this type.
	/// </summary>
	public static bool CanCovertTo<T>([NotNullWhen(returnValue: true)] this string? value)
        => value.CanCovertTo(typeof(T));

	/// <summary>
	///     Returns the default value of a given type
	/// </summary>
	public static object? GetDefaultValue(this Type? type)
        => type?.IsValueType == true ? RuntimeHelpers.GetUninitializedObject(type) : null;

	/// <summary>
	///     Returns the default value of a given type
	/// </summary>
	public static object? GetDefaultValue<T>() => GetDefaultValue(typeof(T));

	/// <summary>
	///     Determines if the type implements the given interface.
	/// </summary>
	public static bool Implements([NotNullWhen(returnValue: true)] this Type? type,
        [NotNullWhen(returnValue: true)] Type? interfaceType)
        => type is not null && interfaceType?.IsInterface == true && type.GetInterfaces().Any(i => i == interfaceType);

	/// <summary>
	///     Determines if the type implements the given interface.
	/// </summary>
	public static bool Implements<TInterface>([NotNullWhen(returnValue: true)] this Type? type)
        => type.Implements(typeof(TInterface));

	/// <summary>
	///     Determines if the type is derived from the given base type.
	/// </summary>
	public static bool IsDerivedFrom([NotNullWhen(returnValue: true)] this Type? type,
        [NotNullWhen(returnValue: true)] Type? baseType)
        => baseType?.IsAssignableFrom(type) == true;

	/// <summary>
	///     Determines if the type is derived from the given base type.
	/// </summary>
	public static bool IsDerivedFrom<TBaseType>([NotNullWhen(returnValue: true)] this Type? type)
        => type.IsDerivedFrom(typeof(TBaseType));

	/// <summary>
	///     Finds all the derived concrete types of baseType in the given assemblies
	/// </summary>
	public static IList<Type> GetAllDerivedConcreteTypes(this ICollection<Assembly>? assemblies, Type baseType)
    {
        if (assemblies is null)
        {
            return [];
        }

        return assemblies.Where(a => !a.IsDynamic)
            .SelectMany(a => a.GetExportedTypes())
            .Where(t => t is { IsAbstract: false, IsInterface: false } && baseType.IsAssignableFrom(t))
            .ToList();
    }

	/// <summary>
	///     Finds all the derived types of baseType in the given assemblies
	/// </summary>
	public static IList<Type> GetAllDerivedConcreteTypes<TBaseType>(this ICollection<Assembly>? assemblies)
        => assemblies.GetAllDerivedConcreteTypes(typeof(TBaseType));

	/// <summary>
	///     Finds all the derived types of baseType in the given assemblies
	/// </summary>
	public static IList<Type> GetAllDerivedConcreteTypes<TBaseType>(this Assembly assembly)
        => new List<Assembly>
        {
            assembly
        }.GetAllDerivedConcreteTypes<TBaseType>();

	/// <summary>
	///     Finds all the derived types of baseType in the given assemblies
	/// </summary>
	public static IList<Type> GetAllDerivedConcreteTypes(this Assembly assembly, Type baseType)
        => new List<Assembly>
        {
            assembly
        }.GetAllDerivedConcreteTypes(baseType);

	/// <summary>
	///     Determines if the type is derived from the given base type or implements the given interface.
	/// </summary>
	public static bool IsDerivedFromOrImplements([NotNullWhen(returnValue: true)] this Type? type,
        [NotNullWhen(returnValue: true)] Type? baseType)
        => baseType?.IsInterface == true ? type.Implements(baseType) : type.IsDerivedFrom(baseType);

	/// <summary>
	///     Determines if the type is derived from the given base type or implements the given interface.
	/// </summary>
	public static bool IsDerivedFromOrImplements<TBaseType>([NotNullWhen(returnValue: true)] this Type? type)
        => type.IsDerivedFromOrImplements(typeof(TBaseType));

	/// <summary>
	///     Determines if the type is an instance of a generic type.
	/// </summary>
	public static bool IsDerivedFromGenericType([NotNullWhen(returnValue: true)] this Type? type,
        [NotNullWhen(returnValue: true)] Type? genericType)
    {
        var typeTmp = type;

        while (typeTmp != null)
        {
            if (typeTmp.IsGenericType && typeTmp.GetGenericTypeDefinition() == genericType)
            {
                return true;
            }

            typeTmp = typeTmp.BaseType;
        }

        return false;
    }

	/// <summary>
	///     Gets the underlying type if the type is Nullable, otherwise just returns the type.
	/// </summary>
	public static Type? GetTrueType([NotNullIfNotNull(nameof(type))] this Type? type)
    {
        if (type is null)
        {
            return null;
        }

        return IsDerivedFromGenericType(type, typeof(Nullable<>)) ? type.GetGenericArguments()[0] : type;
    }

	/// <summary>
	///     Gets the fields that are of the specified type.
	/// </summary>
	public static IList<FieldInfo> GetFieldsDerivedFrom<T>(this Type? type, BindingFlags flags)
    {
        var fieldsOf = new List<FieldInfo>();

        if (type is null)
        {
            return fieldsOf;
        }

        fieldsOf.AddRange(type.GetFields(flags).Where(fld => fld.FieldType.IsDerivedFrom<T>()));

        return fieldsOf;
    }

	/// <summary>
	///     Gets the properties that are of the specified type.
	/// </summary>
	public static IList<PropertyInfo> GetPropertiesDerivedFrom<T>(this Type? type, BindingFlags flags)
    {
        var fieldsOf = new List<PropertyInfo>();

        if (type is null)
        {
            return fieldsOf;
        }

        fieldsOf.AddRange(type.GetProperties(flags).Where(fld => fld.PropertyType.IsDerivedFrom<T>()));

        return fieldsOf;
    }

	/// <summary>
	///     Determines if the type supports null.
	/// </summary>
	/// <param name="type"></param>
	/// <returns></returns>
	public static bool IsNullable([NotNullWhen(returnValue: true)] this Type? type)
        => type != null && (!type.IsValueType || Nullable.GetUnderlyingType(type) != null);

	/// <summary>
	///     Determines if the type has the specified attribute.
	/// </summary>
	public static bool HasAttribute<TAttribute>([NotNullWhen(returnValue: true)] this Type? type, bool inherited = true)
        => type?.GetTypeInfo().GetCustomAttributes(inherited).OfType<TAttribute>().Any() == true;

	/// <summary>
	///     Determines if the type !IsAbstract and !IsInterface.
	/// </summary>
	public static bool IsConcreteType([NotNullWhen(returnValue: true)] this Type? type)
        => type?.GetTypeInfo() is { IsAbstract: false, IsInterface: false };

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
        => prop?.GetCustomAttributes(typeof(T), inherit).GetAttributeType<T>();

	/// <summary>
	///     Gets the specified attribute from the type.
	/// </summary>
	public static T? GetAttribute<T>(this Type? type, bool inherit)
        where T : Attribute
        => type?.GetCustomAttributes(typeof(T), inherit).GetAttributeType<T>();

	/// <summary>
	///     Gets the specified attribute for the assembly.
	/// </summary>
	public static T? GetAttribute<T>(this Assembly? asm)
        where T : Attribute
        => asm?.GetCustomAttributes(typeof(T), inherit: false).GetAttributeType<T>();

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
            return (obj as Assembly)?.GetCustomAttributes(typeof(T), inherit: false).GetAttributeType<T>();
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
            .GetAttributeType<T>();

    private static T? GetAttributeType<T>(this object[]? attributes)
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
}