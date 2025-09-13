using FluentValidation;
using Riber.Domain.Constants;

namespace Riber.Application.Features.Companies.Queries.GetCompanyById;

internal sealed class GetCompanyByIdQueryValidator : AbstractValidator<GetCompanyByIdQuery>
{
    public GetCompanyByIdQueryValidator()
    {
        RuleFor(x => x.CompanyId)
            .NotEmpty()
            .WithMessage(ErrorMessage.Invalid.CompanyId);
    }
}