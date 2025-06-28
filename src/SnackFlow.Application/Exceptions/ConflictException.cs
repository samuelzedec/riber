using System.Net;

namespace SnackFlow.Application.Exceptions;

public class ConflictException(string message) 
    : ApplicationException(message, (int)HttpStatusCode.Conflict);