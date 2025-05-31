using ChefControl.Domain.CompanyContext.Enums;
using ChefControl.Domain.CompanyContext.ObjectValues.CompanyName;
using ChefControl.Domain.CompanyContext.ObjectValues.TaxId;
using ChefControl.Domain.SharedContext.Abstractions;
using ChefControl.Domain.SharedContext.Entities;
using ChefControl.Domain.SharedContext.ObjectValues.Email;
using ChefControl.Domain.SharedContext.ObjectValues.Phone;

namespace ChefControl.Domain.CompanyContext.Entities;

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

    public void UpdateContactInfo(
        string? email = null, 
        string? phone = null)
    {
        if(!string.IsNullOrWhiteSpace(email))
            Email = Email.Create(email);
        
        if(!string.IsNullOrWhiteSpace(phone))
            Phone = Phone.Create(phone);
        
        UpdateEntity();
    }

    public void UpdateTradingName(string? tradingName = null)
    {
        if(!string.IsNullOrWhiteSpace(tradingName))
            CompanyName = CompanyName.Create(CompanyName.Name, tradingName);
        
        UpdateEntity();
    }
    
    #endregion
}