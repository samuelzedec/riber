using SnackFlow.Domain.Abstractions;
using SnackFlow.Domain.Enums;
using SnackFlow.Domain.ValueObjects.CompanyName;
using SnackFlow.Domain.ValueObjects.Email;
using SnackFlow.Domain.ValueObjects.Phone;
using SnackFlow.Domain.ValueObjects.TaxId;

namespace SnackFlow.Domain.Entities;

public sealed class Company : BaseEntity, IAggregateRoot
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
        string name,
        string tradingName,
        string taxId,
        string email,
        string phone,
        ECompanyType type) : base(Guid.CreateVersion7())
    {
        Name = CompanyName.Create(name, tradingName);
        TaxId = TaxId.Create(taxId, type);
        Email = Email.Create(email);
        Phone = Phone.Create(phone);
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
        string name,
        string tradingName,
        string taxId,
        string email,
        string phone,
        ECompanyType type
    ) => new(name, tradingName, taxId, email, phone, type);

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

    public void UpdateTradingName(string tradingName)
        => Name = CompanyName.Create(Name.Corporate, tradingName);
    
    #endregion
}