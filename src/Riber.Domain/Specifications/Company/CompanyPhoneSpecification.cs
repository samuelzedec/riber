using System.Linq.Expressions;
using Riber.Domain.Specifications.Core;

namespace Riber.Domain.Specifications.Company;

public sealed class CompanyPhoneSpecification(string phone)
    : Specification<Entities.Company.Company>
{
    public override Expression<Func<Entities.Company.Company, bool>> ToExpression()
        => entity => entity.Phone.Value == phone;
}