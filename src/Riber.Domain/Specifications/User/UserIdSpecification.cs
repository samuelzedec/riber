using System.Linq.Expressions;
using Riber.Domain.Specifications.Core;

namespace Riber.Domain.Specifications.User;

public sealed class UserIdSpecification(Guid id) 
    : Specification<Entities.User>
{
    public override Expression<Func<Entities.User, bool>> ToExpression()
        => user => user.Id == id;
}