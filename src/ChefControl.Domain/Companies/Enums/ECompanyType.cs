using System.ComponentModel;

namespace ChefControl.Domain.Companies.Enums;

public enum ECompanyType
{
    [Description("Pessoa Física")]
    IndividualWithCpf = 1,
    [Description("Pessoa Jurídica")]
    LegalEntityWithCnpj = 2
}