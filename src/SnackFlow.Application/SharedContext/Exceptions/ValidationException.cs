using System.Net;
using SnackFlow.Application.SharedContext.Abstractions;

namespace SnackFlow.Application.SharedContext.Exceptions;

public class ValidationException(IEnumerable<ValidationError> errors) 
    : ApplicationException("One or more validation errors occurred", (int)HttpStatusCode.BadRequest)
{
    public IEnumerable<ValidationError> Errors => errors;
}
