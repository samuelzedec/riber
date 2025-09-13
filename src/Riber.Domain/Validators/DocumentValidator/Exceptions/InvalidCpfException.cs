using Riber.Domain.Exceptions;

namespace Riber.Domain.Validators.DocumentValidator.Exceptions;

public class InvalidCpfException(string message) 
    : DomainException(message);