using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.Mvc;
using SnackFlow.Application.Common;
using SnackFlow.Application.Features.Auths.Commands.Login;
using SnackFlow.Application.Features.Auths.Queries.GetAuthenticatedUserQuery;

namespace SnackFlow.Api.Controllers;

[ApiController]
[Produces("application/json")]
[Route("api/v{version:apiVersion}/auth")] 
[ApiVersion("1.0")]
public sealed class AuthController(IMediator mediator) : ControllerBase
{
    [HttpPost("login")]
    [RequestTimeout(("fast"))]
    [ProducesResponseType<Result<LoginCommandResponse>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Login(
        [FromBody] LoginCommand command,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(command, cancellationToken);
        return Ok(response);
    }

    [HttpGet("me")]
    [Authorize]
    [RequestTimeout(("fast"))]
    [ProducesResponseType<Result<GetAuthenticatedUserQueryResponse>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAuthenticatedUser(CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetAuthenticatedUserQuery(), cancellationToken);
        return Ok(response);
    }
}