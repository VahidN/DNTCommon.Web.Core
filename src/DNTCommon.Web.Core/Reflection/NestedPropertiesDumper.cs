namespace DNTCommon.Web.Core;

/// <summary>
///     A helper class for dumping nested property values
/// </summary>
public class NestedPropertiesDumper
{
    private readonly List<PropertyData> _result = [];
    private int _index;

    /// <summary>
    ///     Dumps Nested Property Values
    /// </summary>
    /// <param name="data">an instance object</param>
    /// <param name="parent">parent object's name</param>
    /// <param name="dumpLevel">how many levels should be searched</param>
    /// <returns>Nested Property Values List</returns>
    public IList<PropertyData>? DumpPropertyValues(object? data, string? parent = "", int dumpLevel = 2)
    {
        if (data is null || parent is null)
        {
            return null;
        }

        var propertyGetters = QuickReflection.Instance.GetGetterDelegates(data.GetType());

        foreach (var propertyGetter in propertyGetters)
        {
            var dataValue = propertyGetter.GetterFunc(data);
            var name = string.Format(CultureInfo.InvariantCulture, format: "{0}{1}", parent, propertyGetter.Name);
            var propertyType = propertyGetter.PropertyType;
            var underlyingType = Nullable.GetUnderlyingType(propertyType);
            var isNullable = underlyingType is not null;

            if (isNullable)
            {
                propertyType = underlyingType;
            }

            if (dataValue is null)
            {
                var nullDisplayText = propertyGetter.MemberInfo.GetNullDisplayTextAttribute();

                if (isNullable && string.IsNullOrWhiteSpace(nullDisplayText))
                {
                    nullDisplayText = null;
                }

                _result.Add(new PropertyData
                {
                    PropertyName = name,
                    PropertyValue = nullDisplayText,
                    PropertyIndex = _index++,
                    PropertyType = propertyType
                });
            }
            else if (propertyType?.GetTypeInfo().IsEnum == true)
            {
                var enumValue = ((Enum)dataValue).GetEnumStringValue();

                _result.Add(new PropertyData
                {
                    PropertyName = name,
                    PropertyValue = enumValue,
                    PropertyIndex = _index++,
                    PropertyType = propertyType
                });
            }
            else if (propertyType.IsNestedProperty())
            {
                _result.Add(new PropertyData
                {
                    PropertyName = name,
                    PropertyValue = dataValue,
                    PropertyIndex = _index++,
                    PropertyType = propertyType
                });

                if (parent.Split(separator: '.').Length > dumpLevel)
                {
                    continue;
                }

                DumpPropertyValues(dataValue, $"{name}.", dumpLevel);
            }
            else
            {
                _result.Add(new PropertyData
                {
                    PropertyName = name,
                    PropertyValue = dataValue,
                    PropertyIndex = _index++,
                    PropertyType = propertyType
                });
            }
        }

        return _result;
    }
}
