using Microsoft.AspNetCore.Mvc;
using Riber.Application.Common;

namespace Riber.Api.Extensions;

public static class ResultExtension
{
    public static IActionResult ToHttpResult<T>(this Result<T> result)
        => new ObjectResult(result) { StatusCode = (int)result.StatusCode };
}