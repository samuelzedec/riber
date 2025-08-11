using System.Net;

namespace SnackFlow.Application.Exceptions;

public sealed class BadRequestException(string message) 
    : ApplicationException(message, (int)HttpStatusCode.BadRequest);