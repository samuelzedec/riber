using System.ComponentModel;

namespace SnackFlow.Domain.Enums;

public enum EEmailTemplate
{
    [Description("welcome.html")]
    Welcome = 1,
    
    [Description("account-activation.html")]
    AccountActivation = 2,
    
    [Description("password-reset.html")]
    PasswordReset = 3,
    
    [Description("email-verification.html")]
    EmailVerification = 4,
    
    [Description("report-send.html")]
    ReportSend = 5,
    
    [Description("login-notification.html")]
    LoginNotification = 6,
    
    [Description("system-maintenance.html")]
    SystemMaintenance = 7
}