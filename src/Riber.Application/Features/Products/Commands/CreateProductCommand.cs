// using Riber.Application.Abstractions.Commands;
//
// namespace Riber.Application.Features.Products.Commands;
//
// public sealed record CreateProductCommand(
//     string Name,
//     string Description,
//     decimal Price,
//     Guid CategoryId,
//     Stream? ImageStream = null,
//     string ImageName = "",
//     string ImageContent = ""
// ) : ICommand<CreateProductCommandResponse>;