using System.Linq.Expressions;
using Riber.Domain.Specifications.Core;

namespace Riber.Domain.Specifications.Company;

public sealed class CorporateNameSpecification(string corporateName)
    : Specification<Entities.Company.Company>
{
    public override Expression<Func<Entities.Company.Company, bool>> ToExpression()
        => entity => entity.Name.Corporate == corporateName;
}