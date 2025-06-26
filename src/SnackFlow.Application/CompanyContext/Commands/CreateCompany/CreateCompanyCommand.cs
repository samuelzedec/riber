using SnackFlow.Application.SharedContext.Abstractions.Commands;
using SnackFlow.Domain.CompanyContext.Enums;

namespace SnackFlow.Application.CompanyContext.Commands.CreateCompany;

public sealed record CreateCompanyCommand(
    string Name,
    string TradingName,
    string TaxId,
    string Email,
    string Phone,
    ECompanyType Type
) : ICommand<CreateCompanyResponse>;