using SnackFlow.Domain.SharedContext.Exceptions;

namespace SnackFlow.Domain.SharedContext.Validators.DocumentValidator.Exceptions;

public class InvalidCpfException(string message) 
    : DomainException(message);