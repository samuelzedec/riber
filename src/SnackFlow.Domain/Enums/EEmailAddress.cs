using System.ComponentModel;

namespace SnackFlow.Domain.Enums;

public enum EEmailAddress
{
    [Description("noreply@snackflow.app")]
    NoReply = 1,
    [Description("contact@snackflow.app")]
    Contact = 2,
    [Description("security@snackflow.app")]
    Security = 3,
    [Description("support@snackflow.app")]
    Support = 4
}