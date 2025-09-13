using System.Net;

namespace Riber.Application.Exceptions;

public sealed class NotFoundException(string message)
    : ApplicationException(message, (int)HttpStatusCode.NotFound);
