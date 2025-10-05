using Riber.Domain.Abstractions;
using Riber.Domain.Abstractions.ValueObjects;
using Riber.Domain.Constants.Messages.Entities;
using Riber.Domain.Entities.Tenants;
using Riber.Domain.Exceptions;
using Riber.Domain.ValueObjects.Money;

namespace Riber.Domain.Entities;

public sealed class Product
    : TenantEntity, IAggregateRoot, IHasUnitPrice
{
    #region Properties

    public string Name { get; private set; }
    public string Description { get; private set; }
    public Money UnitPrice { get; private set; }
    public Guid CategoryId { get; private set; }
    public bool IsActive { get; private set; }
    public string? ImageUrl { get; private set; }

    #endregion

    #region Navigation Properties

    public Company Company { get; private set; } = null!;
    public ProductCategory Category { get; private set; } = null!;

    #endregion

    #region Constructors

    private Product() : base(Guid.Empty)
    {
        Name = string.Empty;
        Description = string.Empty;
        UnitPrice = null!;
        CategoryId = Guid.Empty;
        IsActive = false;
        CompanyId = Guid.Empty;
    }

    private Product(
        string name,
        string description,
        decimal price,
        Guid categoryId,
        Guid companyId,
        string? imageUrl = null) : base(Guid.CreateVersion7())
    {
        Name = name;
        Description = description;
        UnitPrice = Money.CreatePrice(price);
        CategoryId = categoryId;
        CompanyId = companyId;
        ImageUrl = imageUrl;
        IsActive = true;
    }

    #endregion

    #region Factories

    public static Product Create(
        string name,
        string description,
        decimal price,
        Guid categoryId,
        Guid companyId,
        string? imageUrl = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ProductNameNullException(ProductErrors.NameEmpty);

        if (string.IsNullOrWhiteSpace(description))
            throw new ProductDescriptionNullException(ProductErrors.DescriptionEmpty);

        if (categoryId == Guid.Empty)
            throw new IdentifierNullException(ProductErrors.InvalidCategory);

        if (companyId == Guid.Empty)
            throw new IdentifierNullException(ProductErrors.InvalidCategory);

        return new Product(name, description, price, categoryId, companyId, imageUrl);
    }

    #endregion

    #region Business Methods

    public void UpdateDetails(string name, string description, decimal price)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ProductNameNullException(ProductErrors.NameEmpty);

        if (string.IsNullOrWhiteSpace(description))
            throw new ProductDescriptionNullException(ProductErrors.DescriptionEmpty);

        Name = name;
        Description = description;
        UnitPrice = Money.CreatePrice(price);
    }

    public void ChangeCategory(Guid categoryId)
    {
        if (categoryId == Guid.Empty)
            throw new IdentifierNullException(ProductErrors.InvalidCategory);

        CategoryId = categoryId;
    }

    public void UpdateImage(string? imageUrl) => ImageUrl = imageUrl;
    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;

    #endregion
}