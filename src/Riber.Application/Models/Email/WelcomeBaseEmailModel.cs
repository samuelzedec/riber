namespace Riber.Application.Models.Email;

public sealed record WelcomeBaseEmailModel(
    string Name,
    string To,
    string Subject,
    string TemplatePath
) : BaseEmailModel(To, Subject, TemplatePath);