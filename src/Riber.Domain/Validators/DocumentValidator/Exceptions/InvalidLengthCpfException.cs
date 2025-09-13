using Riber.Domain.Exceptions;

namespace Riber.Domain.Validators.DocumentValidator.Exceptions;

public class InvalidLengthCpfException(string message) 
    : DomainException(message);