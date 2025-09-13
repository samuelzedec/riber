using System.ComponentModel;

namespace Riber.Domain.Enums;

public enum BusinessPosition
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