using System.Net;

namespace SnackFlow.Application.Exceptions;

public class NotFoundException(string message)
    : ApplicationException(message, (int)HttpStatusCode.NotFound);
