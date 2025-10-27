using Riber.Application.Abstractions.Commands;

namespace Riber.Application.Features.Products.Commands;

public sealed record CreateProductCommandResponse(
    Guid ProductId,
    string Name,
    string Description,
    decimal Price,
    string ImageName
) : ICommandResponse;