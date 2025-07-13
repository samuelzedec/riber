using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.Mvc;
using SnackFlow.Application.Common;
using SnackFlow.Application.Features.Companies.Commands.CreateCompany;
using SnackFlow.Application.Features.Companies.Commands.UpdateCompany;
using SnackFlow.Application.Features.Companies.Queries.GetCompanyById;

namespace SnackFlow.Api.Controllers;

[ApiController]
[Route("api/company")]
public class CompanyController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [Authorize]
    [RequestTimeout(("standard"))]
    [ProducesResponseType<Result<CreateCompanyResponse>>(StatusCodes.Status201Created)]
    [ProducesResponseType<Result<CreateCompanyResponse>>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCompany(
        [FromBody] CreateCompanyCommand command,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(command, cancellationToken);
        return Created($"/api/company/{response.Value.CompanyId}", response);
    }
    
    [HttpPut]
    [Authorize]
    [RequestTimeout("standard")]
    [ProducesResponseType<Result<UpdateCompanyResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<Result<UpdateCompanyResponse>>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<Result<UpdateCompanyResponse>>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCompany(
        [FromBody] UpdateCompanyCommand command,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(command, cancellationToken);
        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    [Authorize]
    [RequestTimeout("standard")]
    [ProducesResponseType<Result<GetCompanyByIdResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<Result<GetCompanyByIdResponse>>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<Result<GetCompanyByIdResponse>>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCompanyById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetCompanyByIdQuery(id), cancellationToken);
        return Ok(response);
    }
}