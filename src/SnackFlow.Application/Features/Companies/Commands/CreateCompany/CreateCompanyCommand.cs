using SnackFlow.Application.Abstractions.Commands;
using SnackFlow.Domain.Entities;
using SnackFlow.Domain.Enums;

namespace SnackFlow.Application.Features.Companies.Commands.CreateCompany;

public sealed record CreateCompanyCommand(
    string CorporateName,
    string FantasyName,
    string TaxId,
    string Email,
    string Phone,
    TaxIdType Type
) : ICommand<CreateCompanyCommandResponse>
{
    public static Company ToCompany(CreateCompanyCommand command)
        => Company.Create(
            command.CorporateName,
            command.FantasyName,
            command.TaxId,
            command.Email,
            command.Phone,
            command.Type
        );
}