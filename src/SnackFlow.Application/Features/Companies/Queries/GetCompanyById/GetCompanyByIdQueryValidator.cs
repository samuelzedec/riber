using FluentValidation;
using SnackFlow.Domain.Constants;

namespace SnackFlow.Application.Features.Companies.Queries.GetCompanyById;

public sealed class GetCompanyByIdQueryValidator : AbstractValidator<GetCompanyByIdQuery>
{
    public GetCompanyByIdQueryValidator()
    {
        RuleFor(x => x.CompanyId)
            .NotEmpty()
            .WithMessage(ErrorMessage.Invalid.IdIsNull);
    }
}