using Riber.Application.Abstractions.Commands;

namespace Riber.Application.Features.ProductCategories.Commands;

public sealed record CreateProductCategoryCommandResponse(
    Guid ProductCategoryId,
    string Code,
    string Name
) : ICommandResponse;