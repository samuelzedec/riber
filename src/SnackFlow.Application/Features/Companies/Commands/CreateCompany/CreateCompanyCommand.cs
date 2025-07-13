using SnackFlow.Application.Abstractions.Commands;
using SnackFlow.Domain.Enums;

namespace SnackFlow.Application.Features.Companies.Commands.CreateCompany;

public sealed record CreateCompanyCommand(
    string CorporateName,
    string FantasyName,
    string TaxId,
    string Email,
    string Phone,
    ETaxIdType Type
) : ICommand<CreateCompanyCommandResponse>;