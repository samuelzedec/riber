namespace SnackFlow.Application.DTOs.Email;

public abstract record BaseEmailDTO(
    string To,
    string Subject,
    string Audience,
    string Template
);