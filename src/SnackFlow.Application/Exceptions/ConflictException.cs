using System.Net;

namespace SnackFlow.Application.Exceptions;

public sealed class ConflictException(string message) 
    : ApplicationException(message, (int)HttpStatusCode.Conflict);