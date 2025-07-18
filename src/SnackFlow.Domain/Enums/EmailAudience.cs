using System.ComponentModel;

namespace SnackFlow.Domain.Enums;

public enum EmailAudience
{
    [Description("company")]
    Company = 1,
    [Description("user")]
    User = 2
}