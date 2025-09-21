using System.Linq.Expressions;
using Riber.Domain.Specifications.Core;

namespace Riber.Domain.Specifications.User;

public sealed class UserTaxIdSpecification(string taxId)
    : Specification<Entities.User>
{
    public override Expression<Func<Entities.User, bool>> ToExpression()
        => entity => entity.TaxId.Value == taxId;
}