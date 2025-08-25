using SnackFlow.Domain.Abstractions;
using SnackFlow.Domain.Abstractions.ValueObjects;
using SnackFlow.Domain.Enums;
using SnackFlow.Domain.ValueObjects.CompanyName;
using SnackFlow.Domain.ValueObjects.Email;
using SnackFlow.Domain.ValueObjects.Phone;
using SnackFlow.Domain.ValueObjects.TaxId;

namespace SnackFlow.Domain.Entities;

public sealed class Company 
    : BaseEntity, IAggregateRoot, IHasCompanyName, IHasTaxId, IHasEmail, IHasPhone
{
    #region Properties

    public CompanyName Name { get; private set; }
    public TaxId TaxId { get; private set; }
    public Email Email { get; private set; }
    public Phone Phone { get; private set; }

    #endregion

    #region Constructors
    
    private Company() : base(Guid.Empty) 
    {
        Name = null!;
        TaxId = null!;
        Email = null!;
        Phone = null!;
    }

    private Company(
        CompanyName name,
        TaxId taxId,
        Email email,
        Phone phone) : base(Guid.CreateVersion7())
    {
        Name = name;
        TaxId = taxId;
        Email = email;
        Phone = phone;
    }

    #endregion
    
    #region Factories

    public static Company Create(
        string corporateName,
        string fantasyName,
        string taxId,
        string email,
        string phone,
        TaxIdType type
    ) => new(
        CompanyName.Create(corporateName, fantasyName),
        TaxId.Create(taxId, type),
        Email.Create(email),
        Phone.Create(phone)
    );

    public static Company Create(
        CompanyName name,
        TaxId taxId,
        Email email,
        Phone phone
    ) => new(name, taxId, email, phone);

    #endregion

    #region Methods
    
    public void UpdateEmail(string email)
        => Email = Email.Create(email);

    public void UpdatePhone(string phone)
        => Phone = Phone.Create(phone);

    public void UpdateFantasyName(string fantasyName)
        => Name = CompanyName.Create(Name.Corporate, fantasyName);
    
    #endregion
}