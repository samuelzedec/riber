using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SnackFlow.Application.Common;
using SnackFlow.Application.Features.Companies.Commands.CreateCompany;

namespace SnackFlow.Api.Controllers;

[ApiController]
[Route("api/company")]
public class CompanyController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<Result<CreateCompanyResponse>>(StatusCodes.Status201Created)]
    [ProducesResponseType<Result<CreateCompanyResponse>>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCompany(
        CreateCompanyCommand command,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(command, cancellationToken);
        return Created($"/api/company/{response.Value.CompanyId}", response);
    }
}