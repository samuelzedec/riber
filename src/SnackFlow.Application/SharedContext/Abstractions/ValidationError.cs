namespace SnackFlow.Application.SharedContext.Abstractions;

public sealed record ValidationError(string PropertyName, string ErrorMessage);