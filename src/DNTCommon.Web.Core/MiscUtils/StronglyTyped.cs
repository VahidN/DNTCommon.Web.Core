using System.Linq.Expressions;

namespace DNTCommon.Web.Core;

/// <summary>
///     Get the string name and type of property or field.
/// </summary>
public class StronglyTyped : ExpressionVisitor
{
    private Stack<string>? _stack;

    /// <summary>
    ///     Get the string name and type of property or field.
    /// </summary>
    public static string PropertyPath<TEntity>(Expression<Func<TEntity, object?>>? expression, string separator)
        => expression is null ? string.Empty : new StronglyTyped().Name(expression, separator);

    /// <summary>
    ///     Get the string name and type of property or field.
    /// </summary>
    public string Path(Expression expression, string separator)
    {
        _stack = new Stack<string>();
        Visit(expression);

        return _stack.Aggregate((s1, s2) => $"{s1}{separator}{s2}");
    }

    /// <summary>
    ///     Visits the children of the System.Linq.Expressions.MemberExpression.
    /// </summary>
    protected override Expression VisitMember(MemberExpression node)
    {
        ArgumentNullException.ThrowIfNull(node);

        _stack?.Push(node.Member.Name);

        return base.VisitMember(node);
    }

    /// <summary>
    ///     Get the string name and type of property or field.
    /// </summary>
    public string Name<TEntity>(Expression<Func<TEntity, object?>> expression, string separator)
        => Path(expression, separator);
}