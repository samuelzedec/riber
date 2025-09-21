using Microsoft.Extensions.Logging;
using Riber.Application.Abstractions.Commands;
using Riber.Application.Abstractions.Services;
using Riber.Application.Common;
using Riber.Application.Exceptions;
using Riber.Domain.Constants;
using Riber.Domain.Entities;
using Riber.Domain.Repositories;
using Riber.Domain.Specifications.ProductCategory;
using Riber.Domain.Specifications.Tenants;

namespace Riber.Application.Features.ProductCategories.Commands;

internal sealed class CreateProductCategoryCommandHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUserService,
    ILogger<CreateProductCategoryCommandHandler> logger) 
    : ICommandHandler<CreateProductCategoryCommand, CreateProductCategoryCommandResponse>
{
    private readonly Guid _companyId = currentUserService.GetCompanyId();
    
    public async ValueTask<Result<CreateProductCategoryCommandResponse>> Handle(CreateProductCategoryCommand command, CancellationToken cancellationToken)
    {
        try
        {
            await ValidateCode(command.Code);
            var codeNormalized = command.Code.ToUpperInvariant();
            var category = ProductCategory.Create(
                code: codeNormalized,
                name: command.Name,
                description: command.Description,
                companyId: _companyId
            );
            
            await unitOfWork.Products.CreateCategoryAsync(category, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return new CreateProductCategoryCommandResponse(
                category.Id,
                codeNormalized,
                category.Name
            );
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ErrorMessage.Exception.Unexpected(ex.GetType().Name, ex.Message));
            throw;
        }
    }
    
    private async Task ValidateCode(string code)
    {
        var specification = new TenantSpecification<ProductCategory>(_companyId)
            .And(new ProductCategoryCodeSpecification(code));

        if(await unitOfWork.Products.GetCategoryAsync(specification) is not null)
            throw new BadRequestException(ErrorMessage.Product.CategoryCodeExist);
    }
}