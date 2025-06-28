using SnackFlow.Domain.Exceptions;

namespace SnackFlow.Domain.Validators.DocumentValidator.Exceptions;

public class InvalidCpfException(string message) 
    : DomainException(message);