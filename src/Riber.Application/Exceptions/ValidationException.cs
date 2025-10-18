using System.Net;
using Riber.Application.Common;

namespace Riber.Application.Exceptions;

public sealed class ValidationException(IEnumerable<ValidationError> messages) : Exception
{
    public IEnumerable<ValidationError> Messages => messages;
    public static int Code => (int)HttpStatusCode.BadRequest;
}