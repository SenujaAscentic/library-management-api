using Library.Application.Abstractions.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Library.Api.Common.ErrorHandling;

public static class ErrorExtensions
{
    public static IResult ToProblemDetails(this Error error, HttpContext httpContext)
    {
        var (statusCode, title) = error.Type switch
        {
            ErrorType.NotFound => (StatusCodes.Status404NotFound, "Not Found"),
            ErrorType.Conflict => (StatusCodes.Status409Conflict, "Conflict"),
            ErrorType.BusinessRule => (StatusCodes.Status400BadRequest, "Bad Request"),
            _ => (StatusCodes.Status400BadRequest, "Bad Request")
        };

        return ProblemDetailsHelper.Create(httpContext, statusCode, title, error.Message, error.Code);
    }
}