using SnackFlow.Application.Abstractions.Commands;
using SnackFlow.Application.Common;

namespace SnackFlow.Application.Features.Companies.Commands.CreateCompany;

public class CreateCompanyHandler : ICommandHandler<CreateCompanyCommand, CreateCompanyResponse>
{
    public Task<Result<CreateCompanyResponse>> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}