using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.Mvc;
using SnackFlow.Application.Common;
using SnackFlow.Application.Features.Users.Commands.CreateUser;

namespace SnackFlow.Api.Controllers;

[ApiController]
[Produces("application/json")]
[Route("api/v{version:apiVersion}/user")]
[ApiVersion("1.0")]
public sealed class UserController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [AllowAnonymous]
    [RequestTimeout("standard")]
    [ProducesResponseType<Result<CreateUserCommandResponse>>(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateUser(
        [FromBody] CreateUserCommand command,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(command, cancellationToken);
        return Created($"/api/user/{response.Value.Email}", response);
    }
}