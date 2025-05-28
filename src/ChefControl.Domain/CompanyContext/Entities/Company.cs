using ChefControl.Domain.Companies.Enums;
using ChefControl.Domain.Companies.ObjectValues.CompanyName;
using ChefControl.Domain.Companies.ObjectValues.TaxId;
using ChefControl.Domain.Shared.Abstractions;
using ChefControl.Domain.Shared.Entities;

namespace ChefControl.Domain.Companies.Entities;

public sealed class Company : Entity, IAggregateRoot
{
    #region Properties

    public CompanyName Name { get; private set; }
    public CompanyTaxId TaxId { get; private set; }
    public string Email { get; private set; }
    public string Phone { get; private set; }

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
        Name = CompanyName.Create(name, tradingName);
        TaxId = CompanyTaxId.Create(taxId, type);
        Email = email;
        Phone = phone;
    }

    private Company(
        CompanyName name,
        CompanyTaxId taxId,
        string email,
        string phone,
        ECompanyType type) : base(Guid.CreateVersion7())
    {
        Name = name;
        TaxId = taxId;
        Email = email;
        Phone = phone;
    }

    #endregion
}