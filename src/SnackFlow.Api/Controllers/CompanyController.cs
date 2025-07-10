using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.Mvc;
using SnackFlow.Application.Common;
using SnackFlow.Application.Features.Companies.Commands.CreateCompany;
using SnackFlow.Application.Features.Companies.Commands.UpdateCompany;

namespace SnackFlow.Api.Controllers;

[ApiController]
[Route("api/company")]
public class CompanyController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    // [Authorize]
    [RequestTimeout(("standard"))]
    [ProducesResponseType<Result<CreateCompanyResponse>>(StatusCodes.Status201Created)]
    [ProducesResponseType<Result<CreateCompanyResponse>>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCompany(
        CreateCompanyCommand command,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(command, cancellationToken);
        return Created($"/api/company/{response.Value.CompanyId}", response);
    }
    
    [HttpPatch]
    // [Authorize]
    [RequestTimeout("standard")]
    [ProducesResponseType<Result<UpdateCompanyResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<Result<CreateCompanyResponse>>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<Result<UpdateCompanyResponse>>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCompany(
        UpdateCompanyCommand command,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(command, cancellationToken);
        return Ok(response);
    }
}