namespace DNTCommon.Web.Core;

/// <summary>
///     Expressions extensions
/// </summary>
public static class ExpressionsExtensions
{
    /// <summary>
    ///     Sets the value of a nested Expression
    /// </summary>
    public static void SetPropertyValue<TEntity>(this TEntity? targetInstance,
        Expression<Func<TEntity, object?>> func,
        object? value)
    {
        var (info, memberExp) = GetPropertyInfo(func);
        var target = GetTarget(memberExp.Expression, targetInstance);
        info?.SetValue(target, value, index: null);
    }

    private static object? GetTarget<TEntity>(Expression? expr, TEntity? targetInstance)
    {
        switch (expr?.NodeType)
        {
            case ExpressionType.Parameter:
                return targetInstance;
            case ExpressionType.MemberAccess:
                var mex = (MemberExpression)expr;

                if (mex.Member is not PropertyInfo pi)
                {
                    throw new InvalidOperationException();
                }

                var target = GetTarget(mex.Expression, targetInstance);

                return pi.GetValue(target, index: null);
            default:
                throw new InvalidOperationException();
        }
    }

    /// <summary>
    ///     Gets the DisplayName of the provided expression
    /// </summary>
    public static string GetDisplayName<TEntity>(this Expression<Func<TEntity, object?>>? expression)
    {
        var (_, memberExp) = GetPropertyInfo(expression);
        var displayAttribute = memberExp.Member.GetCustomAttribute<DisplayAttribute>();

        return displayAttribute is null ? string.Empty : displayAttribute.Name ?? memberExp.Member.Name;
    }

    /// <summary>
    ///     Gets the DisplayName of the provided expression
    /// </summary>
    public static string GetDisplayName<TEntity>(this Expression<Func<TEntity>>? expression)
    {
        var memberExp = expression.GetMemberExpression() ??
                        throw new ArgumentException(
                            $"The expression doesn't indicate a valid property. [ {expression} ]", nameof(expression));

        var displayAttribute = memberExp.Member.GetCustomAttribute<DisplayAttribute>();

        return displayAttribute is null ? string.Empty : displayAttribute.Name ?? memberExp.Member.Name;
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

    /// <summary>
    ///     Gets the PropertyInfo of the provided expression
    /// </summary>
    public static (PropertyInfo Info, MemberExpression MemberExp) GetPropertyInfo<TEntity>(
        this Expression<Func<TEntity, object?>>? expression)
        => expression?.Body switch
        {
            null => throw new ArgumentNullException(nameof(expression)),
            UnaryExpression { Operand: MemberExpression memberExp } => ((PropertyInfo)memberExp.Member, memberExp),
            MemberExpression memberExp => ((PropertyInfo)memberExp.Member, memberExp),
            _ => throw new ArgumentException($"The expression doesn't indicate a valid property. [ {expression} ]",
                nameof(expression))
        };

    /// <summary>
    ///     Get the actual return type of expr
    /// </summary>
    public static Type? GetObjectType<TEntity>(this Expression<Func<TEntity, object?>>? expression)
    {
        if (expression is null)
        {
            return null;
        }

        return expression.Body.NodeType is ExpressionType.Convert or ExpressionType.ConvertChecked &&
               expression.Body is UnaryExpression unary
            ? unary.Operand.Type
            : expression.Body.Type;
    }
}