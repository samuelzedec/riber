using System.Linq.Expressions;

namespace Riber.Domain.Specifications.Core;

internal sealed class NotSpecification<T>(Specification<T> specification) 
    : Specification<T>
{
    public override Expression<Func<T, bool>> ToExpression()
    {
        var expression = specification.ToExpression();
        return Expression.Lambda<Func<T, bool>>(
            Expression.Not(expression.Body),
            expression.Parameters
        );
    }
}