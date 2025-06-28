using SnackFlow.Domain.Exceptions;

namespace SnackFlow.Domain.Validators.DocumentValidator.Exceptions;

public class InvalidLengthCpfException(string message) 
    : DomainException(message);