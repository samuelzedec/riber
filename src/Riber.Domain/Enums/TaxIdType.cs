using System.ComponentModel;

namespace Riber.Domain.Enums;

public enum TaxIdType
{
    [Description("Pessoa Física")]
    IndividualWithCpf = 1,
    [Description("Pessoa Jurídica")]
    LegalEntityWithCnpj = 2
}