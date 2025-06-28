using SnackFlow.Domain.Exceptions;

namespace SnackFlow.Domain.Validators.DocumentValidator.Exceptions;

public class InvalidLengthCnpjException(string message) 
    : DomainException(message);