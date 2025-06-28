using System.Net;

namespace SnackFlow.Application.Exceptions;

public class RequestTimeoutException(string requestName, TimeSpan elapsedTime)
    : ApplicationException($"Request '{requestName}' timed out after {elapsedTime.TotalSeconds:F1} seconds", (int)HttpStatusCode.RequestTimeout);