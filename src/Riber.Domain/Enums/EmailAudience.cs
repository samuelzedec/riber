using System.ComponentModel;

namespace Riber.Domain.Enums;

public enum EmailAudience
{
    [Description("company")]
    Company = 1,
    [Description("user")]
    User = 2
}