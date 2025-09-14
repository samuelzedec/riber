using System.ComponentModel;

namespace Riber.Domain.Enums;

public enum EmailAddress
{
    [Description("noreply@riber.app")]
    NoReply = 1,
    [Description("contact@sriber.app")]
    Contact = 2,
    [Description("security@riber.app")]
    Security = 3,
    [Description("support@riber.app")]
    Support = 4
}