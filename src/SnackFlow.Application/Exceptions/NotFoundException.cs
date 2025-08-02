using System.Net;

namespace SnackFlow.Application.Exceptions;

public sealed class NotFoundException(string message)
    : ApplicationException(message, (int)HttpStatusCode.NotFound);
