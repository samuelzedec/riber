using System.Net;

namespace Riber.Application.Exceptions;

public sealed class RequestTimeoutException(string requestName, TimeSpan elapsedTime)
    : ApplicationException($"Request '{requestName}' timed out after {elapsedTime.TotalSeconds:F1} seconds", (int)HttpStatusCode.RequestTimeout);