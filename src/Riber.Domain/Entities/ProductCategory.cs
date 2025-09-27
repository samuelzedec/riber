using Riber.Domain.Constants;
using Riber.Domain.Entities.Tenants;
using Riber.Domain.Exceptions;

namespace Riber.Domain.Entities;

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

    public Company Company { get; private set; } = null!;
    public IReadOnlyCollection<Product> ProductsReadOnly => _products.AsReadOnly();

    #endregion

    #region Constructors

    private ProductCategory() : base(Guid.Empty)
    {
        Name = string.Empty;
        Description = string.Empty;
        Code = string.Empty;
        IsActive = false;
        CompanyId = Guid.Empty;
    }

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
            throw new ProductCategoryNameNullException(ErrorMessage.Product.CategoryNameIsNull);
        
        if (string.IsNullOrWhiteSpace(code))
            throw new ProductCategoryCodeNullException(ErrorMessage.Product.CategoryCodeIsNull);
        
        if (companyId == Guid.Empty)
            throw new IdentifierNullException(ErrorMessage.Invalid.CompanyId);
        
        return new ProductCategory(name, description, code, companyId);
    }

    #endregion

    #region Business Methods

    public void UpdateDetails(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ProductCategoryNameNullException(ErrorMessage.Product.CategoryNameIsNull);
        
        Name = name;
        Description = description;
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;

    #endregion
}
