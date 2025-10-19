using Asp.Versioning;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.Mvc;
using Riber.Application.Common;
using Riber.Application.Features.Users.Commands.CreateUser;

namespace Riber.Api.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/user")]
[ApiVersion("1.0")]
[Produces("application/json")]
[Consumes("application/json")]
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
        return Created($"/api/user/{response.Value?.Email}", response);
    }
}