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

    #endregion
    
    #region Operators
    
    public static implicit operator Expression<Func<T, bool>>(Specification<T> specification)
        => specification.ToExpression();
    
    #endregion
}