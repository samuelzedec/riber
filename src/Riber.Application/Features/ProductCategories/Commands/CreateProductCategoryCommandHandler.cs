using System.Net;
using Riber.Application.Abstractions.Commands;
using Riber.Application.Abstractions.Services.Authentication;
using Riber.Application.Common;
using Riber.Domain.Constants.Messages.Common;
using Riber.Domain.Constants.Messages.Entities;
using Riber.Domain.Entities;
using Riber.Domain.Repositories;
using Riber.Domain.Specifications.ProductCategory;
using Riber.Domain.Specifications.Tenants;

namespace Riber.Application.Features.ProductCategories.Commands;

internal sealed class CreateProductCategoryCommandHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUserService)
    : ICommandHandler<CreateProductCategoryCommand, CreateProductCategoryCommandResponse>
{
    public async ValueTask<Result<CreateProductCategoryCommandResponse>> Handle(CreateProductCategoryCommand command,
        CancellationToken cancellationToken)
    {
        var companyId = currentUserService.GetCompanyId();
        if (!companyId.HasValue)
            return Result.Failure<CreateProductCategoryCommandResponse>(CompanyErrors.Invalid);

        if (await ValidateCode(command.Code, companyId.Value, cancellationToken))
            return Result.Failure<CreateProductCategoryCommandResponse>(ConflictErrors.CategoryCode, HttpStatusCode.Conflict);

        var codeNormalized = command.Code.ToUpperInvariant();
        var category = ProductCategory.Create(
            code: codeNormalized,
            name: command.Name,
            description: command.Description,
            companyId: companyId.Value
        );

        await unitOfWork.Products.CreateCategoryAsync(category, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateProductCategoryCommandResponse(
            category.Id,
            codeNormalized,
            category.Name
        );
    }

    private async Task<bool> ValidateCode(
        string code,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        var specification = new TenantSpecification<ProductCategory>(companyId)
            .And(new ProductCategoryCodeSpecification(code));

        return await unitOfWork
            .Products
            .GetCategoryAsync(specification, cancellationToken) is not null;
    }
}