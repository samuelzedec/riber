using System.ComponentModel;

namespace Riber.Domain.Enums;

public enum EmailAddress
{
    [Description("noreply@riberpay.app")]
    NoReply = 1,
    [Description("contact@sriberpay.app")]
    Contact = 2,
    [Description("security@riberpay.app")]
    Security = 3,
    [Description("support@riberpay.app")]
    Support = 4
}