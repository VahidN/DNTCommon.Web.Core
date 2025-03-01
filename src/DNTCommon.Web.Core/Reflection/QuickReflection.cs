using System.Collections.Concurrent;

namespace DNTCommon.Web.Core;

/// <summary>
///     Fast property access, using Reflection.Emit.
/// </summary>
public class QuickReflection
{
    /// <summary>
    ///     It's a lazy loaded thread-safe singleton.
    /// </summary>
    private static readonly Lazy<QuickReflection> QuickReflectionInstance = new(() => new QuickReflection(),
        LazyThreadSafetyMode.ExecutionAndPublication);

    // 'GetOrAdd' call on the dictionary is not thread safe and we might end up creating the GetterInfo more than
    // once. To prevent this Lazy<> is used. In the worst case multiple Lazy<> objects are created for multiple
    // threads but only one of the objects succeeds in creating a GetterInfo.
    private readonly ConcurrentDictionary<Type, Lazy<List<GetterInfo>>> _gettersCache = new();

    private QuickReflection()
    {
    }

    /// <summary>
    ///     Singleton instance of FastReflection.
    /// </summary>
    public static QuickReflection Instance { get; } = QuickReflectionInstance.Value;

    /// <summary>
    ///     Fast property access, using Reflection.Emit.
    /// </summary>
    public IList<GetterInfo> GetGetterDelegates(Type type)
    {
        var getterDelegates = _gettersCache.GetOrAdd(type, typeCache => new Lazy<List<GetterInfo>>(() =>
        {
            var gettersList = new List<GetterInfo>();
            var properties = typeCache.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var index = 0;

            foreach (var property in properties)
            {
                var getterDelegate = CreateGetterPropertyDelegate(typeCache, property, index);
                index++;

                if (getterDelegate is null)
                {
                    continue;
                }

                var info = new GetterInfo
                {
                    Name = property.Name,
                    GetterFunc = getterDelegate,
                    PropertyType = property.PropertyType,
                    MemberInfo = property
                };

                gettersList.Add(info);
            }

            var fields = typeCache.GetFields(BindingFlags.Instance | BindingFlags.Public);

            foreach (var field in fields)
            {
                var getterDelegate = CreateGetterFieldDelegate(typeCache, field);

                if (getterDelegate is null)
                {
                    continue;
                }

                var info = new GetterInfo
                {
                    Name = field.Name,
                    GetterFunc = getterDelegate,
                    PropertyType = field.FieldType,
                    MemberInfo = field
                };

                gettersList.Add(info);
            }

            return gettersList;
        }));

        return getterDelegates.Value;
    }

    private static Func<object, object> CreateGetterFieldDelegate(Type type, FieldInfo fieldInfo)
    {
        var instanceParam = Expression.Parameter(typeof(object), name: "instance");
        var field = Expression.Field(Expression.TypeAs(instanceParam, type), fieldInfo);
        var convertField = Expression.TypeAs(field, typeof(object));

        return Expression.Lambda<Func<object, object>>(convertField, instanceParam).Compile();
    }

    private static Func<object, object> CreateGetterPropertyDelegate(Type type, PropertyInfo propertyInfo, int index)
    {
        var getMethod = propertyInfo.GetMethod ?? throw new InvalidOperationException(
            string.Format(CultureInfo.InvariantCulture, format: "Couldn't get the GetMethod of {0}", type));

        var hasParameter = getMethod.GetParameters().Length != 0;

        var instanceParam = Expression.Parameter(typeof(object), name: "instance");

        var getterExpression = Expression.Convert(
            hasParameter
                ? Expression.Call(Expression.Convert(instanceParam, type), getMethod, Expression.Constant(index))
                : Expression.Call(Expression.Convert(instanceParam, type), getMethod), typeof(object));

        return Expression.Lambda<Func<object, object>>(getterExpression, instanceParam).Compile();
    }
}
