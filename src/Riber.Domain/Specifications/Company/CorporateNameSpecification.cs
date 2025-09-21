using System.Linq.Expressions;
using Riber.Domain.Specifications.Core;

namespace Riber.Domain.Specifications.Company;

public sealed class CorporateNameSpecification(string corporateName)
    : Specification<Entities.Company>
{
    public override Expression<Func<Entities.Company, bool>> ToExpression()
        => entity => entity.Name.Corporate == corporateName;
}