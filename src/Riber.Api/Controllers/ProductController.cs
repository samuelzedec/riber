using Asp.Versioning;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.Mvc;
using Riber.Api.Attributes;
using Riber.Api.Extensions;
using Riber.Api.Requests;
using Riber.Application.Common;
using Riber.Application.Features.Products.Commands;
using Riber.Infrastructure.Settings;

namespace Riber.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v{version:apiVersion}/product")]
[ApiVersion("1.0")]
[Produces("application/json")]
[Consumes("application/json", "multipart/form-data")]
public sealed class ProductController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [RequestSizeLimit(3_145_728)]
    [RequestFormLimits(MultipartBodyLengthLimit = 3_145_728)]
    [RequirePermission(PermissionsSettings.Products.Create)]
    [RequestTimeout("standard")]
    [ProducesResponseType<Result<CreateProductCommandResponse>>(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateProduct(
        [FromForm] CreateProductRequest request,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(request.ToCommand(), cancellationToken);
        return response.ToHttpResult($"/api/product/{response.Value?.ProductId}");
    }
}