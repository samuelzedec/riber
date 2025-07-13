using FluentValidation;
using SnackFlow.Domain.Constants;

namespace SnackFlow.Application.Features.Companies.Queries.GetCompanyById;

public sealed class GetCompanyByIdValidator : AbstractValidator<GetCompanyByIdQuery>
{
    public GetCompanyByIdValidator()
    {
        RuleFor(x => x.CompanyId)
            .NotEmpty()
            .WithMessage(ErrorMessage.Invalid.IdIsNull);
    }
}