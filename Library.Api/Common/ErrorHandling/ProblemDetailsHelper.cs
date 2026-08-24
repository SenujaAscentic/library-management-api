// Library.Api/Common/ErrorHandling/ProblemDetailsHelper.cs
using Microsoft.AspNetCore.Mvc;

namespace Library.Api.Common.ErrorHandling;

public static class ProblemDetailsHelper
{
    public static IResult Create(HttpContext httpContext, int statusCode, string title, string detail, string code)
    {
        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = httpContext.Request.Path
        };

        problemDetails.Extensions["code"] = code;
        problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;

        return Results.Json(problemDetails, statusCode: statusCode, contentType: "application/problem+json");
    }
}