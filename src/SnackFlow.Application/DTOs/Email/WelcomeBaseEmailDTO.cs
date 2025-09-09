namespace SnackFlow.Application.DTOs.Email;

public sealed record WelcomeBaseEmailDTO(
    string Name,
    string To,
    string Subject,
    string TemplatePath
) : BaseEmailDTO(To, Subject, TemplatePath);