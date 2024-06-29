namespace DNTCommon.Web.Core;

/// <summary>
///     DisplayAttribute extensions
/// </summary>
public static class DisplayNameExtensions
{
    /// <summary>
    ///     Gets the DisplayName of the provided expression
    /// </summary>
    public static string GetDisplayName<TEntity>(this Expression<Func<TEntity>>? expression) => expression.GetDisplayName();

    /// <summary>
    ///     Gets the DisplayName of the provided expression
    /// </summary>
    public static string GetDisplayName(this Expression? expression)
    {
        var memberExp = expression.GetMemberExpression();

        if (memberExp is null)
        {
            throw new ArgumentException($"The expression doesn't indicate a valid property. [ {expression} ]",
                nameof(expression));
        }

        var displayAttribute = memberExp.Member.GetCustomAttribute<DisplayAttribute>();

        if (displayAttribute is null)
        {
            return string.Empty;
        }

        return displayAttribute.Name ?? memberExp.Member.Name;
    }

    /// <summary>
    ///     Represents accessing a field or property.
    /// </summary>
    /// <param name="expression"></param>
    /// <returns></returns>
    public static MemberExpression? GetMemberExpression(this Expression? expression)
    {
        switch (expression)
        {
            case MemberExpression memberExpression:
                return memberExpression;
            case LambdaExpression lambdaExpression:
                switch (lambdaExpression.Body)
                {
                    case MemberExpression memExpression:
                        return memExpression;
                    case UnaryExpression unaryExpression:
                        return (MemberExpression)unaryExpression.Operand;
                }

                break;
        }

        return null;
    }
}