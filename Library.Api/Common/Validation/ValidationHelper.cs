using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Library.Api.Common.Validation;

public static class ValidationHelper
{
    public static async Task<IResult?> ValidateAsync<T>(T request, IValidator<T> validator, HttpContext httpContext)
    {
        var result = await validator.ValidateAsync(request);

        if (result.IsValid)
        {
            return null;
        }

        var errors = result.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.ErrorMessage).ToArray());

        return Results.ValidationProblem(
            errors,
            title: "Validation Failed",
            statusCode: StatusCodes.Status400BadRequest,
            extensions: new Dictionary<string, object?>
            {
                ["code"] = "validation_failed",
                ["traceId"] = httpContext.TraceIdentifier
            });
    }
}