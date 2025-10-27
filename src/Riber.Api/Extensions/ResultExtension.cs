using System.Net;
using Microsoft.AspNetCore.Mvc;
using Riber.Application.Common;

namespace Riber.Api.Extensions;

public static class ResultExtension
{
    public static IActionResult ToHttpResult<T>(this Result<T> result, string location = "")
        => result.StatusCode is HttpStatusCode.Created
            ? new CreatedResult(location, result)
            : new ObjectResult(result) { StatusCode = (int)result.StatusCode };
}