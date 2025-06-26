using SnackFlow.Domain.SharedContext.Exceptions;

namespace SnackFlow.Domain.SharedContext.Validators.DocumentValidator.Exceptions;

public class UnsupportedCompanyTypeException(string message) 
    : DomainException(message);