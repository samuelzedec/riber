namespace SnackFlow.Domain.Exceptions;

public sealed class CompanyIsNullException(string message)
    : DomainException(message);