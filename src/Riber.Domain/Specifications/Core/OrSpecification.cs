using System.Linq.Expressions;

namespace Riber.Domain.Specifications.Core;

internal sealed class OrSpecification<T>(
    Specification<T> left,
    Specification<T> right)
    : Specification<T>
{
    public override Expression<Func<T, bool>> ToExpression()
    {
        (Expression leftBody, Expression rightBody, ParameterExpression parameter) = UnifyParameters(left, right);
        return Expression.Lambda<Func<T, bool>>(
            Expression.OrElse(leftBody, rightBody),
            parameter
        );
    }
}