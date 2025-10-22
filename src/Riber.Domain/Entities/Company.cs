using Riber.Domain.Abstractions;
using Riber.Domain.Abstractions.ValueObjects;
using Riber.Domain.Enums;
using Riber.Domain.ValueObjects.CompanyName;
using Riber.Domain.ValueObjects.Email;
using Riber.Domain.ValueObjects.Phone;
using Riber.Domain.ValueObjects.TaxId;

namespace Riber.Domain.Entities;

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
    
#pragma warning disable CS8618, CA1823
    private Company() : base(Guid.Empty) { }
#pragma warning restore CS8618, CA1823

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