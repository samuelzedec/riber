using System.Linq.Expressions;

namespace Riber.Domain.Specifications.Core;

internal sealed class OrSpecification<T>(
    Specification<T> left,
    Specification<T> right)
    : Specification<T>
{
    public override Expression<Func<T, bool>> ToExpression()
    {
        var leftExpression = left.ToExpression();
        var rightExpression = right.ToExpression();

        var parameter = Expression.Parameter(typeof(T), "entity");
        var leftBody = ReplaceParameter(leftExpression.Body, leftExpression.Parameters[0], parameter);
        var rightBody = ReplaceParameter(rightExpression.Body, rightExpression.Parameters[0], parameter);

        return Expression.Lambda<Func<T, bool>>(
            Expression.OrElse(leftBody, rightBody),
            parameter
        );
    }
}