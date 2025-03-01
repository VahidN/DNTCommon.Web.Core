#if NET_9 || NET_8
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace DNTCommon.Web.Core;

/// <summary>
///     This utility class returns the HtmlFieldName of a given ValueExpression with its HtmlFieldPrefix
/// </summary>
public class BlazorHtmlField<T> : InputBase<T>
{
    /// <summary>
    ///     This utility class returns the HtmlFieldName of a given ValueExpression with its HtmlFieldPrefix
    /// </summary>
    public BlazorHtmlField(Expression<Func<T>> valueExpression)
    {
        ValueExpression = valueExpression;
        _ = base.SetParametersAsync(ParameterView.FromDictionary(new Dictionary<string, object?>()));
        HtmlFieldName = NameAttributeValue;
    }

    /// <summary>
    ///     Gets the value to be used for the input's "name" attribute.
    /// </summary>
    public string HtmlFieldName { get; private set; }

    /// <summary>
    ///     Gets the name of the related FieldIdentifier.
    /// </summary>
    public string FieldIdentifierName => !string.IsNullOrWhiteSpace(FieldIdentifier.FieldName)
        ? FieldIdentifier.FieldName
        : Guid.NewGuid().ToString(format: "N");

    /// <summary>
    ///     Parses a string to create an instance of T. Derived classes can override this to change how
    ///     CurrentValueAsString interprets incoming values.
    /// </summary>
    protected override bool TryParseValueFromString(string? value, out T result, out string validationErrorMessage)
        => throw new NotSupportedException();

    /// <summary>
    ///     Signals that the value for the specified field has changed.
    ///     Signals that some aspect of validation state has changed.
    /// </summary>
    public void NotifyFieldChanged(EditContext? editContext)
    {
        if (editContext is null)
        {
            return;
        }

        var fieldIdentifier = FieldIdentifier;

        if (string.IsNullOrWhiteSpace(fieldIdentifier.FieldName))
        {
            return;
        }

        editContext.NotifyFieldChanged(in fieldIdentifier);
        editContext.NotifyValidationStateChanged();
    }
}
#endif
