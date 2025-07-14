using SnackFlow.Domain.Abstractions;
using SnackFlow.Domain.Constants;
using SnackFlow.Domain.Enums;
using SnackFlow.Domain.Exceptions;
using SnackFlow.Domain.ValueObjects.FullName;
using SnackFlow.Domain.ValueObjects.TaxId;

namespace SnackFlow.Domain.Entities;

public class User : BaseEntity, IAggregateRoot
{
    #region Properties

    public FullName FullName { get; private set; }
    public TaxId TaxId { get; private set; }
    public EBusinessPosition Position { get; private set; }
    public bool IsActive { get; private set; }
    public Guid? CompanyId { get; private set; }
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
        EBusinessPosition position,
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
        EBusinessPosition position,
        Guid? companyId = null
    ) => new(FullName.Create(fullName), TaxId.Create(taxId, ETaxIdType.IndividualWithCpf), position, companyId);

    #endregion

    #region Methods

    public void Activate()
    {
        if (CompanyId is null)
            throw new CompanyIsNullException(ErrorMessage.Invalid.IdIsNull);
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