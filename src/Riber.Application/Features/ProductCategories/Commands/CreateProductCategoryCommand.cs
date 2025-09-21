using Riber.Application.Abstractions.Commands;

namespace Riber.Application.Features.ProductCategories.Commands;

public sealed record CreateProductCategoryCommand(
    string Name,
    string Description,
    string Code
) : ICommand<CreateProductCategoryCommandResponse>;