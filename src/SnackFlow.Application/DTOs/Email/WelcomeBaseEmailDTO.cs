namespace SnackFlow.Application.DTOs.Email;

public sealed class WelcomeBaseEmailDTO : BaseEmailDTO
{
    public required string Name { get; set; } = string.Empty;
}