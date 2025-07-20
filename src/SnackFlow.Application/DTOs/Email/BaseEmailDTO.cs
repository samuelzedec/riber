namespace SnackFlow.Application.DTOs.Email;

public abstract class BaseEmailDTO
{
    public required string To { get; set; } 
    public required string Subject { get; set; }
    public required string Audience { get; set; }
    public required string Template { get; set; }
}