using SnackFlow.Domain.Abstractions;
using SnackFlow.Domain.Enums;
using SnackFlow.Domain.ValueObjects.CompanyName;
using SnackFlow.Domain.ValueObjects.Email;
using SnackFlow.Domain.ValueObjects.Phone;
using SnackFlow.Domain.ValueObjects.TaxId;

namespace SnackFlow.Domain.Entities;

public sealed class Company : Entity, IAggregateRoot
{
    #region Properties

    public CompanyName CompanyName { get; private set; }
    public CompanyTaxId TaxId { get; private set; }
    public Email Email { get; private set; }
    public Phone Phone { get; private set; }

    #endregion

    #region Constructors
    
    private Company(
        string name,
        string tradingName,
        string taxId,
        string email,
        string phone,
        ECompanyType type) : base(Guid.CreateVersion7())
    {
        CompanyName = CompanyName.Create(name, tradingName);
        TaxId = CompanyTaxId.Create(taxId, type);
        Email = Email.Create(email);
        Phone = Phone.Create(phone);
    }

    private Company(
        CompanyName companyName,
        CompanyTaxId taxId,
        Email email,
        Phone phone) : base(Guid.CreateVersion7())
    {
        CompanyName = companyName;
        TaxId = taxId;
        Email = email;
        Phone = phone;
    }

    #endregion

    #region  ORM Constructor

    private Company() : base(Guid.Empty) 
    {
        CompanyName = null!;
        TaxId = null!;
        Email = null!;
        Phone = null!;
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
        CompanyTaxId taxId,
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
        => CompanyName = CompanyName.Create(CompanyName.Name, tradingName);
    
    #endregion
}