using ChefControl.Domain.CompanyContext.Enums;
using ChefControl.Domain.CompanyContext.ObjectValues.CompanyName;
using ChefControl.Domain.CompanyContext.ObjectValues.TaxId;
using ChefControl.Domain.SharedContext.Abstractions;
using ChefControl.Domain.SharedContext.Entities;

namespace ChefControl.Domain.CompanyContext.Entities;

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
        string phone) : base(Guid.CreateVersion7())
    {
        Name = name;
        TaxId = taxId;
        Email = email;
        Phone = phone;
    }

    #endregion
}