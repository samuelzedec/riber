using System.Net;
using Riber.Application.Common;

namespace Riber.Application.Exceptions;

public sealed class ValidationException(IEnumerable<ValidationError> errors) 
    : ApplicationException("One or more validation errors occurred", (int)HttpStatusCode.BadRequest)
{
    public IEnumerable<ValidationError> Errors => errors;
}
