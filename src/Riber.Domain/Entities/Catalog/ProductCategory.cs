using Riber.Domain.Constants.Messages.Entities;
using Riber.Domain.Entities.Catalog.Exceptions;
using Riber.Domain.Entities.Tenants;
using Riber.Domain.Exceptions;

namespace Riber.Domain.Entities.Catalog;

public sealed class ProductCategory
    : TenantEntity
{
    #region Properties

    public string Name { get; private set; }
    public string Description { get; private set; }
    public string Code { get; private set; }
    public bool IsActive { get; private set; }
    private readonly List<Product> _products = [];

    #endregion

    #region Navigation Properties

    public Company.Company Company { get; private set; } = null!;
    public IReadOnlyCollection<Product> ProductsReadOnly => _products.AsReadOnly();

    #endregion

    #region Constructors

#pragma warning disable CS8618, CA1823
    private ProductCategory() : base(Guid.Empty) { }
#pragma warning restore CS8618, CA1823

    private ProductCategory(
        string name,
        string description,
        string code,
        Guid companyId) : base(Guid.CreateVersion7())
    {
        Name = name.Trim();
        Description = description;
        Code = code.ToUpperInvariant();
        CompanyId = companyId;
        IsActive = true;
    }

    #endregion

    #region Factories

    public static ProductCategory Create(
        string name,
        string description,
        string code,
        Guid companyId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ProductCategoryNameNullException(CategoryErrors.NameEmpty);

        if (string.IsNullOrWhiteSpace(code))
            throw new ProductCategoryCodeNullException(CategoryErrors.CodeEmpty);

        return companyId == Guid.Empty
            ? throw new IdentifierNullException(CompanyErrors.Invalid)
            : new ProductCategory(name, description, code, companyId);
    }

    #endregion

    #region Business Methods

    public void UpdateDetails(string name, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ProductCategoryNameNullException(CategoryErrors.NameEmpty);

        Name = name;
        if (description is not null)
            Description = description;
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;

    #endregion
}