using SnackFlow.Application.Abstractions.Commands;
using SnackFlow.Domain.Enums;

namespace SnackFlow.Application.Features.Companies.Commands.CreateCompany;

public sealed record CreateCompanyCommand(
    string Name,
    string TradingName,
    string TaxId,
    string Email,
    string Phone,
    ECompanyType Type
) : ICommand<CreateCompanyResponse>;