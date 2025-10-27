using Asp.Versioning;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.Mvc;
using Riber.Api.Attributes;
using Riber.Api.Extensions;
using Riber.Api.Requests.Company;
using Riber.Application.Common;
using Riber.Application.Features.Companies.Commands.CreateCompanyWithAdmin;
using Riber.Application.Features.Companies.Commands.UpdateCompany;
using Riber.Application.Features.Companies.Queries.GetCompanyById;
using Riber.Infrastructure.Settings;

namespace Riber.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v{version:apiVersion}/company")]
[ApiVersion("1.0")]
[Produces("application/json")]
[Consumes("application/json")]
public sealed class CompanyController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [AllowAnonymous]
    [RequestTimeout(("standard"))]
    [ProducesResponseType<Result<CreateCompanyWithAdminCommandResponse>>(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateCompany(
        [FromBody] CreateCompanyWithAdminCommand withAdminCommand,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(withAdminCommand, cancellationToken);
        return response.ToHttpResult($"/api/company/{response.Value?.CompanyId}");
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType<Result<UpdateCompanyCommandResponse>>(StatusCodes.Status200OK)]
    [RequirePermission(PermissionsSettings.Companies.Read, PermissionsSettings.Companies.Update)]
    [RequestTimeout("standard")]
    public async Task<IActionResult> UpdateCompany(
        [FromRoute] Guid id,
        [FromBody] UpdateCompanyRequest request,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(request.ToCommand(id), cancellationToken);
        return response.ToHttpResult();
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<Result<GetCompanyByIdQueryResponse>>(StatusCodes.Status200OK)]
    [RequirePermission(PermissionsSettings.Companies.Read)]
    [RequestTimeout("standard")]
    public async Task<IActionResult> GetCompanyById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetCompanyByIdQuery(id), cancellationToken);
        return response.ToHttpResult();
    }
}