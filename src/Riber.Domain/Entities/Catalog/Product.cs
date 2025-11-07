using Riber.Domain.Abstractions;
using Riber.Domain.Abstractions.ValueObjects;
using Riber.Domain.Constants.Messages.Entities;
using Riber.Domain.Entities.Catalog.Exceptions;
using Riber.Domain.Entities.Tenants;
using Riber.Domain.Exceptions;
using Riber.Domain.ValueObjects.Money;

namespace Riber.Domain.Entities.Catalog;

public sealed class Product
    : TenantEntity, IAggregateRoot, IHasUnitPrice, IHasXmin
{
    #region Properties

    public string Name { get; private set; }
    public string Description { get; private set; }
    public Money UnitPrice { get; private set; }
    public Guid CategoryId { get; private set; }
    public bool IsActive { get; private set; }
    public Guid? ImageId { get; private set; }
    public uint XminCode { get; set; }

    #endregion

    #region Navigation Properties

    public Company.Company Company { get; private set; } = null!;
    public ProductCategory Category { get; private set; } = null!;
    public Image? Image { get; private set; }

    #endregion

    #region Constructors

#pragma warning disable CS8618, CA1823
    private Product() : base(Guid.Empty) { }
#pragma warning restore CS8618, CA1823

    private Product(
        string name,
        string description,
        decimal price,
        Guid categoryId,
        Guid companyId,
        Guid? imageId = null) : base(Guid.CreateVersion7())
    {
        Name = name;
        Description = description;
        UnitPrice = Money.CreatePrice(price);
        CategoryId = categoryId;
        CompanyId = companyId;
        ImageId = imageId;
        IsActive = true;
        Image = null;
    }

    #endregion

    #region Factories

    public static Product Create(
        string name,
        string description,
        decimal price,
        Guid categoryId,
        Guid companyId,
        Guid? imageId = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ProductNameNullException(ProductErrors.NameEmpty);

        if (string.IsNullOrWhiteSpace(description))
            throw new ProductDescriptionNullException(ProductErrors.DescriptionEmpty);

        if (categoryId == Guid.Empty)
            throw new IdentifierNullException(ProductErrors.InvalidCategory);

        return companyId == Guid.Empty
            ? throw new IdentifierNullException(ProductErrors.InvalidCompany)
            : new Product(name, description, price, categoryId, companyId, imageId);
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

    public void UpdateImage(Guid? imageId) => ImageId = imageId;
    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;

    #endregion
}