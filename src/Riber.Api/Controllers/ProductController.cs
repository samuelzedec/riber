using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.Mvc;
using Riber.Api.Attributes;
using Riber.Api.Requests;
using Riber.Application.Common;
using Riber.Application.Features.Product.Commands;
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
    [RequirePermission(PermissionsSettings.Products.Create)]
    [RequestTimeout("standard")]
    [ProducesResponseType<Result<CreateProductCommandResponse>>(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateProduct(
        [FromForm] CreateProductRequest request,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new CreateProductCommand(
            Name: request.Name,
            Description: request.Description,
            Price: request.Price,
            CategoryId: request.CategoryId,
            ImageStream: request.Image?.OpenReadStream(),
            ImageName: request.Image?.FileName ?? string.Empty,
            ImageContent: request.Image?.ContentType ?? string.Empty
        ), cancellationToken);
        return Created($"api/product/{response.Value.ProductId}", response);
    }
}