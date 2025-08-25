using FluentValidation;
using SnackFlow.Domain.Constants;

namespace SnackFlow.Application.Features.Companies.Queries.GetCompanyById;

internal sealed class GetCompanyByIdQueryValidator : AbstractValidator<GetCompanyByIdQuery>
{
    public GetCompanyByIdQueryValidator()
    {
        RuleFor(x => x.CompanyId)
            .NotEmpty()
            .WithMessage(ErrorMessage.Invalid.CompanyId);
    }
}