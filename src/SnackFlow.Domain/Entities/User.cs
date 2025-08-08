using SnackFlow.Domain.Abstractions;
using SnackFlow.Domain.Constants;
using SnackFlow.Domain.Enums;
using SnackFlow.Domain.Exceptions;
using SnackFlow.Domain.ValueObjects.FullName;
using SnackFlow.Domain.ValueObjects.TaxId;

namespace SnackFlow.Domain.Entities;

public sealed class User : BaseEntity, IAggregateRoot
{
    #region Properties

    public Guid ApplicationUserId { get; private set; }
    public FullName FullName { get; private set; }
    public TaxId TaxId { get; private set; }
    public BusinessPosition Position { get; private set; }
    public bool IsActive { get; private set; }
    public string PublicToken { get; private set; }
    public Guid? CompanyId { get; private set; }
    public Company Company { get; private set; } = null!;

    #endregion

    #region Constructors

    public User() : base(Guid.Empty)
    {
        FullName = null!;
        TaxId = null!;
        CompanyId = null!;
        ApplicationUserId = Guid.Empty;
        IsActive = false;
        PublicToken = string.Empty;
    }
    
    private User(
        FullName fullName,
        TaxId taxId,
        Guid applicationUserId,
        BusinessPosition position,
        Guid? companyId) : base(Guid.CreateVersion7())
    {
        FullName = fullName;
        TaxId = taxId;
        Position = position;
        ApplicationUserId = applicationUserId;
        CompanyId = companyId;
        IsActive = false;
        PublicToken = GeneratePublicCode();
    }

    #endregion

    #region Factories

    public static User Create(
        string fullName,
        string taxId,
        BusinessPosition position,
        Guid applicationUserId,
        Guid? companyId = null
    ) => new(
        FullName.Create(fullName),
        TaxId.Create(taxId, TaxIdType.IndividualWithCpf),
        applicationUserId,
        position,
        companyId
    );

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

    #region Operators

    public static implicit operator string(User user)
        => user.ToString();

    #endregion

    #region Overrides

    public override string ToString()
        => ApplicationUserId.ToString();

    #endregion
}