using ChefControl.Application.SharedContext.Abstractions.Commands;
using ChefControl.Application.SharedContext.Results;

namespace ChefControl.Application.CompanyContext.Commands.CreateCompany;

public class CreateCompanyHandler : ICommandHandler<CreateCompanyCommand, CreateCompanyResponse>
{
    public Task<Result<CreateCompanyResponse>> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}