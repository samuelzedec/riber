using Mediator;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.Mvc;
using SnackFlow.Application.Common;
using SnackFlow.Application.Features.Users.Commands.CreateUser;

namespace SnackFlow.Api.Controllers;

[ApiController]
[Route("api/user")]
public class UserController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [RequestTimeout("standard")]
    [ProducesResponseType<Result<CreateUserCommandResponse>>(StatusCodes.Status201Created)]
    [ProducesResponseType<Result<CreateUserCommandResponse>>(StatusCodes.Status400BadRequest)]

    public async Task<IActionResult> CreateUser(
        [FromBody] CreateUserCommand command,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(command, cancellationToken);
        return Ok(response);
    }
}