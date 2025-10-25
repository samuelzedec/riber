using Riber.Application.Features.Products.Commands;

namespace Riber.Api.Requests;

public sealed record CreateProductRequest(
    string Name,
    string Description,
    decimal Price,
    Guid CategoryId,
    IFormFile? Image = null)
{
    public CreateProductCommand ToCommand()
        => new(
            Name: Name,
            Description: Description,
            Price: Price,
            CategoryId: CategoryId,
            ImageStream: Image?.OpenReadStream(),
            ImageName: Image?.FileName ?? string.Empty,
            ImageContent: Image?.ContentType ?? string.Empty
        );
}