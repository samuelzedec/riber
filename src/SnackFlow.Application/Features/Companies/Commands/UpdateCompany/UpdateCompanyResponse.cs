using SnackFlow.Application.Abstractions.Commands;

namespace SnackFlow.Application.Features.Companies.Commands.UpdateCompany;

public record UpdateCompanyResponse(
    string TradingName,
    string Email,
    string Phone,
    string Type
) : ICommandResponse;