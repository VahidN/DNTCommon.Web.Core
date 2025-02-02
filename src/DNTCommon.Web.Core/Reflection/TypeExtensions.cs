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
        => assemblies is null
            ? []
            : (IList<Type>)assemblies.Where(a => !a.IsDynamic)
                .SelectMany(a => a.GetExportedTypes())
                .Where(t => t is { IsAbstract: false, IsInterface: false } && baseType.IsAssignableFrom(t))
                .ToList();

	/// <summary>
	///     Finds all the derived concrete types of baseType in the given assemblies
	/// </summary>
	public static IList<Type> GetAllDerivedConcreteTypes(this ICollection<Assembly>? assemblies,
        params ICollection<Type>? baseTypes)
        => assemblies is null || baseTypes is null
            ? []
            : (IList<Type>)assemblies.Where(a => !a.IsDynamic)
                .SelectMany(a => a.GetExportedTypes())
                .Where(t => t is { IsAbstract: false, IsInterface: false } &&
                            baseTypes.Any(baseType => baseType.IsAssignableFrom(t)))
                .ToList();

	/// <summary>
	///     Finds all the derived concrete types of baseType in the given assemblies
	/// </summary>
	public static IList<Type> GetAllDerivedConcreteTypes(this Assembly assembly, params ICollection<Type>? baseTypes)
        => new List<Assembly>
        {
            assembly
        }.GetAllDerivedConcreteTypes(baseTypes);

	/// <summary>
	///     Finds all the concrete types in the given assemblies
	/// </summary>
	public static IList<Type> GetAllConcreteTypes(this ICollection<Assembly>? assemblies)
        => assemblies is null
            ? []
            : (IList<Type>)assemblies.Where(a => !a.IsDynamic)
                .SelectMany(a => a.GetExportedTypes())
                .Where(t => t is { IsAbstract: false, IsInterface: false })
                .ToList();

	/// <summary>
	///     Finds all the exported types of the given assembly
	/// </summary>
	public static IList<Type> GetAllConcreteTypes(this Assembly assembly)
        => new List<Assembly>
        {
            assembly
        }.GetAllConcreteTypes();

	/// <summary>
	///     Finds all the derived types of baseType in the given assemblies
	/// </summary>
	public static IList<Type> GetAllDerivedConcreteTypes<TBaseType>(this ICollection<Assembly>? assemblies)
        => assemblies.GetAllDerivedConcreteTypes(typeof(TBaseType));

	/// <summary>
	///     Finds all the derived types of baseType in the given assembly
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
	///     Determines if the type !IsAbstract and !IsInterface.
	/// </summary>
	public static bool IsConcreteType([NotNullWhen(returnValue: true)] this Type? type)
        => type?.GetTypeInfo() is { IsAbstract: false, IsInterface: false };

	/// <summary>
	///     Indicates whether the current Type represents an enumeration.
	/// </summary>
	public static bool IsEnum(this Type type) => type.GetTypeInfo().IsEnum;

	/// <summary>
	///     Gets the Assembly in which the type is declared.
	/// </summary>
	public static Assembly Assembly(this Type type) => type.GetTypeInfo().Assembly;

	/// <summary>
	///     The Type from which the current Type directly inherits, or null if the current Type represents the Object class or
	///     an interface.
	/// </summary>
	public static Type? BaseType(this Type type) => type.GetTypeInfo().BaseType;

	/// <summary>
	///     Determines whether the current Type derives from the specified Type.
	/// </summary>
	public static bool IsSubclassOf(this Type type, Type parent) => type.GetTypeInfo().IsSubclassOf(parent);

	/// <summary>
	///     Gets a value indicating whether the Type is abstract and must be overridden.
	/// </summary>
	public static bool IsAbstract(this Type type) => type.GetTypeInfo().IsAbstract;

	/// <summary>
	///     Gets a value indicating whether the current type is a generic type.
	/// </summary>
	public static bool IsGenericType(this Type type) => type.GetTypeInfo().IsGenericType;

	/// <summary>
	///     searches for the fields defined for the current Type, using the specified binding constraints.
	/// </summary>
	public static FieldInfo[] GetFields(this Type type, BindingFlags flags) => type.GetTypeInfo().GetFields(flags);

	/// <summary>
	///     An object representing the interface with the specified name, implemented or inherited by the current Type, if
	///     found; otherwise, null.
	/// </summary>
	public static Type? GetInterface(this Type type, string name) => type.GetTypeInfo().GetInterface(name);

	/// <summary>
	///     An array of Type objects that represent the type arguments of a generic type. Returns an empty array if the current
	///     type is not a generic type.
	/// </summary>
	public static Type[] GetGenericArguments(this Type type) => type.GetTypeInfo().GetGenericArguments();

	/// <summary>
	///     An array of objects representing all properties of the current Type that match the specified binding constraints.
	///     -or- An empty array of type PropertyInfo, if the current Type does not have properties, or if none of the
	///     properties match the binding constraints.
	/// </summary>
	public static PropertyInfo[] GetProperties(this Type type, BindingFlags flags)
        => type.GetTypeInfo().GetProperties(flags);

	/// <summary>
	///     Determines if a type is numeric.  Nullable numeric types are considered numeric.
	/// </summary>
	/// <remarks>
	///     Boolean is not considered numeric.
	/// </remarks>
	public static bool IsNumericType(this Type? type)
    {
        if (type == null)
        {
            return false;
        }

        switch (Type.GetTypeCode(type))
        {
            case TypeCode.Byte:
            case TypeCode.Decimal:
            case TypeCode.Double:
            case TypeCode.Int16:
            case TypeCode.Int32:
            case TypeCode.Int64:
            case TypeCode.SByte:
            case TypeCode.Single:
            case TypeCode.UInt16:
            case TypeCode.UInt32:
            case TypeCode.UInt64:
                return true;
            case TypeCode.Object:
                if (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    return IsNumericType(Nullable.GetUnderlyingType(type));
                }

                return false;
        }

        return false;
    }

	/// <summary>
	///     Determines whether this type is a NestedProperty?
	/// </summary>
	/// <param name="type"></param>
	/// <returns></returns>
	public static bool IsNestedProperty(this Type? type)
    {
        if (type is null)
        {
            return false;
        }

        var typeInfo = type.GetTypeInfo();
        var assemblyFullName = typeInfo.Assembly.FullName;

        if (assemblyFullName?.StartsWith(value: "mscorlib", StringComparison.OrdinalIgnoreCase) == true ||
            assemblyFullName?.StartsWith(value: "System.Private.CoreLib", StringComparison.OrdinalIgnoreCase) == true)
        {
            return false;
        }

        return (typeInfo.IsClass || typeInfo.IsInterface) && !typeInfo.IsValueType &&
               !string.IsNullOrEmpty(type.Namespace) &&
               !type.Namespace.StartsWith(value: "System.", StringComparison.OrdinalIgnoreCase);
    }
}