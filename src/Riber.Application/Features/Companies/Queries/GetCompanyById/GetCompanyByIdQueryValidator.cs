using FluentValidation;
using Riber.Domain.Constants.Messages.Entities;

namespace Riber.Application.Features.Companies.Queries.GetCompanyById;

internal sealed class GetCompanyByIdQueryValidator : AbstractValidator<GetCompanyByIdQuery>
{
    public GetCompanyByIdQueryValidator()
    {
        RuleFor(x => x.CompanyId)
            .NotEmpty()
            .WithMessage(CompanyErrors.Invalid);
    }
}