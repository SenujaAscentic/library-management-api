using FluentValidation;
using Library.Api.Contracts.Common;

namespace Library.Api.Common.Validation;

public static class ValidationHelper
{
    public static async Task<IResult?> ValidateAsync<T>(
        T request,
        IValidator<T> validator)
    {
        var result =
            await validator.ValidateAsync(request);

        if (result.IsValid)
        {
            return null;
        }

        return Results.BadRequest(
            new ValidationErrorResponse(
                400,
                "Validation failed",
                result.Errors
                    .Select(error =>
                        new ValidationError(
                            error.PropertyName,
                            error.ErrorMessage))
                    .ToList()));
    }
}