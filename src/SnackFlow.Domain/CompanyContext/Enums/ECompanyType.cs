using System.ComponentModel;

namespace SnackFlow.Domain.CompanyContext.Enums;

public enum ECompanyType
{
    [Description("Pessoa Física")]
    IndividualWithCpf = 1,
    [Description("Pessoa Jurídica")]
    LegalEntityWithCnpj = 2
}