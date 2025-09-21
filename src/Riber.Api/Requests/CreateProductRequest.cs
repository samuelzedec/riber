namespace Riber.Api.Requests;

public sealed record CreateProductRequest(
    string Name,
    string Description,
    decimal Price,
    Guid CategoryId,
    IFormFile? Image = null
);