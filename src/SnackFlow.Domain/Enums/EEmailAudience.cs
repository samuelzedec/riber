using System.ComponentModel;

namespace SnackFlow.Domain.Enums;

public enum EEmailAudience
{
    [Description("company")]
    Company = 1,
    [Description("user")]
    User = 2
}