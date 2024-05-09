using System.Linq.Expressions;

namespace DNTCommon.Web.Core;

/// <summary>
///     Expression Helpers
/// </summary>
public static class LambdaExtensions
{
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