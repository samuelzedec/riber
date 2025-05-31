namespace ChefControl.Application.SharedContext.Exceptions;

public class ValidationException(IEnumerable<ValidationError> errors) : Exception
{
    public IEnumerable<ValidationError> Errors 
        => errors;
}
