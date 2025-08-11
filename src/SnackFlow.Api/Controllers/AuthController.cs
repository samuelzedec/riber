using Mediator;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.Mvc;
using SnackFlow.Application.Common;
using SnackFlow.Application.Features.Auths.Commands.Login;

namespace SnackFlow.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(IMediator mediator) : ControllerBase
{
    [HttpPost("login")]
    [RequestTimeout(("standard"))]
    [ProducesResponseType<Result<LoginCommandResponse>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Login(
        [FromBody] LoginCommand command,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(command, cancellationToken);
        return Ok(response);
    }
}