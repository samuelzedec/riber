using ChefControl.Application.SharedContext.Abstractions.Commands;
using ChefControl.Domain.CompanyContext.Enums;

namespace ChefControl.Application.CompanyContext.Commands.CreateCompany;

public sealed record CreateCompanyCommand(
    string Name,
    string TradingName,
    string TaxId,
    string Email,
    string Phone,
    ECompanyType Type
) : ICommand<CreateCompanyResponse>;