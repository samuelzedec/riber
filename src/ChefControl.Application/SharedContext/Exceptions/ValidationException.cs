using System.Net;
using ChefControl.Application.SharedContext.Abstractions;

namespace ChefControl.Application.SharedContext.Exceptions;

public class ValidationException(IEnumerable<ValidationError> errors) 
    : ApplicationException("One or more validation errors occurred", (int)HttpStatusCode.BadRequest)
{
    public IEnumerable<ValidationError> Errors => errors;
}
