using System.Linq.Expressions;

namespace Riber.Domain.Specifications.Core;

public abstract class Specification<T>
{
    #region Abstract Methods

    public abstract Expression<Func<T, bool>> ToExpression();

    #endregion

    #region Methods

    public bool IsSatisfiedBy(T entity)
    {
        var predicate = ToExpression().Compile();
        return predicate(entity);
    }

    public Specification<T> And(Specification<T> specification)
        => new AndSpecification<T>(this, specification);

    public Specification<T> Or(Specification<T> specification)
        => new OrSpecification<T>(this, specification);

    public Specification<T> Not()
        => new NotSpecification<T>(this);

    protected static Expression ReplaceParameter(
        Expression body,
        ParameterExpression oldParameter,
        ParameterExpression newParameter)
        => new ReplacerParameter(oldParameter, newParameter).Visit(body);

    protected static (Expression left, Expression right, ParameterExpression parameter) UnifyParameters(
        Specification<T> left,
        Specification<T> right)
    {
        var leftExpression = left.ToExpression();
        var rightExpression = right.ToExpression();

        var parameter = Expression.Parameter(typeof(T), "entity");
        var leftBody = ReplaceParameter(leftExpression.Body, leftExpression.Parameters[0], parameter);
        var rightBody = ReplaceParameter(rightExpression.Body, rightExpression.Parameters[0], parameter);

        return (leftBody, rightBody, parameter);
    }

    #endregion

    #region Operators

    public static implicit operator Expression<Func<T, bool>>(Specification<T> specification)
        => specification.ToExpression();

    #endregion
}