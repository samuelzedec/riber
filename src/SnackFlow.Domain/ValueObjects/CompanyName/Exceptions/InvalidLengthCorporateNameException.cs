using SnackFlow.Domain.Exceptions;

namespace SnackFlow.Domain.ValueObjects.CompanyName.Exceptions;

public class InvalidLengthCorporateNameException(string message)
    : DomainException(message);