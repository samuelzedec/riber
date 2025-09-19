using Riber.Domain.Abstractions;
using Riber.Domain.Abstractions.ValueObjects;
using Riber.Domain.Constants;
using Riber.Domain.Entities.Tenants;
using Riber.Domain.Enums;
using Riber.Domain.Exceptions;
using Riber.Domain.ValueObjects.FullName;
using Riber.Domain.ValueObjects.TaxId;

namespace Riber.Domain.Entities;

public sealed class User 
    : OptionalTenantEntity, IAggregateRoot, IHasFullName, IHasTaxId
{
    #region Properties

    public FullName FullName { get; private set; }
    public TaxId TaxId { get; private set; }
    public BusinessPosition Position { get; private set; }
    public bool IsActive { get; private set; }

    #endregion

    #region Navigation Properties

    public Company Company { get; private set; } = null!;

    #endregion

    #region Constructors

    public User() : base(Guid.Empty)
    {
        FullName = null!;
        TaxId = null!;
        CompanyId = null!;
        IsActive = false;
    }
    
    private User(
        FullName fullName,
        TaxId taxId,
        BusinessPosition position,
        Guid? companyId) : base(Guid.CreateVersion7())
    {
        FullName = fullName;
        TaxId = taxId;
        Position = position;
        CompanyId = companyId;
        IsActive = false;
    }

    #endregion

    #region Factories

    public static User Create(
        string fullName,
        string taxId,
        BusinessPosition position,
        Guid? companyId = null
    ) => new(FullName.Create(fullName), TaxId.Create(taxId, TaxIdType.IndividualWithCpf), position, companyId);

    #endregion

    #region Methods

    public void Activate()
    {
        if (CompanyId is null)
            throw new IdentifierNullException(ErrorMessage.Invalid.CompanyId);
        IsActive = true;
    }
    
    public void Disable()
        => IsActive = false;
    
    public void AddCompany(Guid companyId)
        => CompanyId = companyId;
    
    public void RemoveFromCompany()
    {
        CompanyId = null;
        Disable();
    }

    #endregion
}