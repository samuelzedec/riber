using Asp.Versioning;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.Mvc;
using Riber.Api.Attributes;
using Riber.Application.Common;
using Riber.Application.Features.ProductCategories.Commands;
using Riber.Infrastructure.Settings;

namespace Riber.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v{version:apiVersion}/product-category")]
[ApiVersion("1.0")]
[Produces("application/json")]
[Consumes("application/json")]
public sealed class ProductCategoryController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [RequestTimeout("standard")]
    [ProducesResponseType<Result<CreateProductCategoryCommandResponse>>(StatusCodes.Status201Created)]
    [RequirePermission(PermissionsSettings.Categories.Create)]
    public async Task<IActionResult> CreateProductCategory(
        [FromBody] CreateProductCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(request, cancellationToken);
        return Ok(response);
    }
}