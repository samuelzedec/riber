namespace SnackFlow.Application.DTOs.Email;

public sealed record WelcomeBaseEmailDTO(
    string Name,
    string To,
    string Subject,
    string Audience,
    string Template
) : BaseEmailDTO(To, Subject, Audience, Template);