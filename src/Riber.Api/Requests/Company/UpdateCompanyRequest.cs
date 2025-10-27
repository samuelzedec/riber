using Riber.Application.Features.Companies.Commands.UpdateCompany;

namespace Riber.Api.Requests.Company;

public sealed record UpdateCompanyRequest(
    string Email,
    string Phone,
    string FantasyName)
{
    public UpdateCompanyCommand ToCommand(Guid companyId) 
        => new(companyId, Email, Phone, FantasyName);
}