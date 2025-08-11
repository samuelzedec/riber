using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.Mvc;
using SnackFlow.Api.Attributes;
using SnackFlow.Application.Common;
using SnackFlow.Application.Features.Companies.Commands.CreateCompanyWithAdmin;
using SnackFlow.Application.Features.Companies.Commands.UpdateCompany;
using SnackFlow.Application.Features.Companies.Queries.GetCompanyById;
using SnackFlow.Infrastructure.Settings;

namespace SnackFlow.Api.Controllers;

[ApiController]
[Produces("application/json")]
[Route("api/company")]
public class CompanyController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [RequestTimeout(("standard"))]
    [ProducesResponseType<Result<CreateCompanyWithAdminCommandResponse>>(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateCompany(
        [FromBody] CreateCompanyWithAdminCommand withAdminCommand,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(withAdminCommand, cancellationToken);
        return Created($"/api/company/{response.Value.CompanyId}", response);
    }
    
    [HttpPut]
    [Authorize(AuthenticationSchemes = nameof(AccessTokenSettings))]
    [ProducesResponseType<Result<UpdateCompanyCommandResponse>>(StatusCodes.Status200OK)]
    [RequirePermission(PermissionsSettings.Companies.Read, PermissionsSettings.Companies.Update)]
    [RequestTimeout("standard")] 
    public async Task<IActionResult> UpdateCompany(
        [FromBody] UpdateCompanyCommand command,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(command, cancellationToken);
        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    [Authorize]
    [ProducesResponseType<Result<GetCompanyByIdQueryResponse>>(StatusCodes.Status200OK)]
    [RequirePermission(PermissionsSettings.Companies.Read)]
    [RequestTimeout("standard")]
    public async Task<IActionResult> GetCompanyById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetCompanyByIdQuery(id), cancellationToken);
        return Ok(response);
    }
}