using Microsoft.Extensions.Logging;
using Riber.Application.Abstractions.Commands;
using Riber.Application.Abstractions.Services;
using Riber.Application.Common;
using Riber.Domain.Constants;
using Riber.Domain.Repositories;

namespace Riber.Application.Features.Product.Commands;

internal sealed class CreateProductCommandHandler(
    IUnitOfWork unitOfWork,
    IImageStorageService imageStorageService,
    ICurrentUserService currentUserService,
    ILogger<CreateProductCommandHandler> logger)
    : ICommandHandler<CreateProductCommand, CreateProductCommandResponse>
{
    public async ValueTask<Result<CreateProductCommandResponse>> Handle(
        CreateProductCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            string? imageUrl = null;
            if (command.ImageStream is not null)
                imageUrl = await imageStorageService.UploadAsync(
                    command.ImageStream,
                    command.ImageName,
                    command.ImageContent
                );

            var product = Domain.Entities.Product.Create(
                name: command.Name,
                description: command.Description,
                price: command.Price,
                categoryId: command.CategoryId,
                currentUserService.GetCompanyId(),
                imageUrl
            );

            await unitOfWork.Products.CreateAsync(product, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return new CreateProductCommandResponse(
                ProductId: product.Id,
                Name: product.Name,
                Description: product.Description,
                Price: product.UnitPrice,
                ImageName: product.ImageUrl ?? string.Empty
            );
        }
        catch (Exception ex)
        {
            logger.LogError(ErrorMessage.Exception.Unexpected(ex.GetType().Name, ex.Message));
            throw;
        }
    }
}