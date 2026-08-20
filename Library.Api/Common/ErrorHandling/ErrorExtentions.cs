using Library.Application.Abstractions.Results;
using Microsoft.AspNetCore.Http;

namespace Library.Api.Common.ErrorHandling;

public static class ErrorExtensions
{
    public static IResult ToProblemDetails(this Error error) => error.Type switch
    {
        ErrorType.NotFound => Microsoft.AspNetCore.Http.Results.NotFound(new { error.Code, error.Message }),
        ErrorType.Conflict => Microsoft.AspNetCore.Http.Results.Conflict(new { error.Code, error.Message }),
        ErrorType.BusinessRule => Microsoft.AspNetCore.Http.Results.BadRequest(new { error.Code, error.Message }),
        _ => Microsoft.AspNetCore.Http.Results.BadRequest(new { error.Code, error.Message })
    };
}