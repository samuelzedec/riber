using SnackFlow.Application.SharedContext.Abstractions.Commands;
using SnackFlow.Application.SharedContext.Results;

namespace SnackFlow.Application.CompanyContext.Commands.CreateCompany;

public class CreateCompanyHandler : ICommandHandler<CreateCompanyCommand, CreateCompanyResponse>
{
    public Task<Result<CreateCompanyResponse>> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}