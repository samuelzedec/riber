using System.ComponentModel;

namespace SnackFlow.Domain.Enums;

public enum EBusinessPosition
{
    [Description("Funcionário")]
    Employee = 1,
    [Description("Gerente")]
    Manager = 2,
    [Description("Diretor")]
    Director = 3,
    [Description("Proprietário")]
    Owner = 4
}