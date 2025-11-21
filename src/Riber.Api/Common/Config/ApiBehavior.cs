using System.Net;
using Microsoft.AspNetCore.Mvc;
using Riber.Application.Common;
using EmptyResult = Microsoft.AspNetCore.Mvc.EmptyResult;

namespace Riber.Api.Common.Config;

internal static class ApiBehavior
{
    extension(IMvcBuilder builder)
    {
        public IMvcBuilder ConfigureInvalidModelStateResponse()
        {
            return builder.ConfigureApiBehaviorOptions(options =>
                options.InvalidModelStateResponseFactory = CreateInvalidModelStateResponse);
        }
    }

    private static IActionResult CreateInvalidModelStateResponse(ActionContext context)
    {
        var errors = context.ModelState
            .Where(e => e.Value?.Errors.Count > 0)
            .ToDictionary(
                kvp => kvp.Key.Replace("$.", "").ToLower(),
                kvp => kvp.Value!.Errors
                    .Select(e => e.ErrorMessage)
                    .ToArray()
            );

        errors.Remove("command", out var command);
        var result = Result.Failure<EmptyResult>(
            command?[0] ?? "Dados de entrada inválidos.",
            HttpStatusCode.BadRequest,
            errors
        );

        return new BadRequestObjectResult(result);
    }
}