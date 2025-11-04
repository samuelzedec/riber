using System.Net;
using Mediator;
using Riber.Application.Abstractions.Services;
using Riber.Application.Abstractions.Services.Authentication;
using Riber.Application.Common;
using Riber.Application.Exceptions;
using Riber.Domain.Constants.Messages.Common;
using Riber.Domain.Constants.Messages.Entities;
using Riber.Domain.Entities;
using Riber.Domain.Events;
using Riber.Domain.Repositories;
using Riber.Domain.Specifications.ProductCategory;

namespace Riber.Application.Features.Products.Commands;

internal sealed class CreateProductCommandHandler(
    IUnitOfWork unitOfWork,
    IImageStorageService imageStorageService,
    ICurrentUserService currentUserService,
    IMediator mediator)
    : Abstractions.Commands.ICommandHandler<CreateProductCommand, CreateProductCommandResponse>
{
    public async ValueTask<Result<CreateProductCommandResponse>> Handle(
        CreateProductCommand command,
        CancellationToken cancellationToken)
    {
        string imageKeyInBucket = string.Empty;
        try
        {
            var companyId = currentUserService.GetCompanyId();
            if (!companyId.HasValue)
                return Result.Failure<CreateProductCommandResponse>(CompanyErrors.Invalid);

            await unitOfWork.BeginTransactionAsync(cancellationToken);
            if (await VerifyCategoryAsync(command.CategoryId))
                return Result.Failure<CreateProductCommandResponse>(NotFoundErrors.Category, HttpStatusCode.NotFound);

            var image = await SaveImageAsync(
                command.ImageStream,
                command.ImageName,
                command.ImageContent,
                cancellationToken
            );

            imageKeyInBucket = image?.ToString() ?? imageKeyInBucket;

            var product = Product.Create(
                name: command.Name,
                description: command.Description,
                price: command.Price,
                categoryId: command.CategoryId,
                companyId.Value,
                image?.Id
            );

            product.RaiseEvent(new GenerateProductEmbeddingsEvent(product.Id));
            await unitOfWork.Products.CreateAsync(product, cancellationToken);
            await unitOfWork.CommitTransactionAsync(cancellationToken);

            return Result.Success(new CreateProductCommandResponse(
                ProductId: product.Id,
                Name: product.Name,
                Description: product.Description,
                Price: product.UnitPrice,
                ImageName: imageKeyInBucket
            ), HttpStatusCode.Created);
        }
        catch (Exception ex) when (ex is not NotFoundException or InternalException)
        {
            if (!string.IsNullOrEmpty(imageKeyInBucket))
                await mediator.Publish(new ProductImageCreationFailedEvent(imageKeyInBucket), cancellationToken);

            await unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
        catch (Exception)
        {
            await unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    private async Task<bool> VerifyCategoryAsync(Guid categoryId)
        => await unitOfWork
            .Products
            .GetCategoryAsync(new ProductCategoryIdSpecification(categoryId)) is null;

    private async Task<Image?> SaveImageAsync(
        Stream? stream,
        string originalName,
        string contentType,
        CancellationToken cancellationToken)
    {
        if (stream is null)
            return null;

        var image = Image.Create(stream.Length, originalName, contentType);
        await imageStorageService.UploadAsync(stream, image, contentType);

        await unitOfWork.Products.CreateImageAsync(image, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return image;
    }
}