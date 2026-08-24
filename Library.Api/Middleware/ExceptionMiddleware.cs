using Library.Api.Common.ErrorHandling;
using Library.Domain.Exceptions;

namespace Library.Api.Middleware;

public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (NotFoundException ex)
        {
            logger.LogWarning(ex, "Not found: {Code} — {Message}", ex.Code, ex.Message);
            await ProblemDetailsHelper.Create(context, StatusCodes.Status404NotFound, "Not Found", ex.Message, ex.Code)
                .ExecuteAsync(context);
        }
        catch (ConflictException ex)
        {
            logger.LogWarning(ex, "Conflict: {Code} — {Message}", ex.Code, ex.Message);
            await ProblemDetailsHelper.Create(context, StatusCodes.Status409Conflict, "Conflict", ex.Message, ex.Code)
                .ExecuteAsync(context);
        }
        catch (BusinessRuleException ex)
        {
            logger.LogWarning(ex, "Business rule rejected: {Code} — {Message}", ex.Code, ex.Message);
            await ProblemDetailsHelper.Create(context, StatusCodes.Status400BadRequest, "Bad Request", ex.Message, ex.Code)
                .ExecuteAsync(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception on {Method} {Path}", context.Request.Method, context.Request.Path);
            await ProblemDetailsHelper.Create(
                    context, StatusCodes.Status500InternalServerError, "Internal Server Error",
                    "An unexpected error occurred. Please contact support and reference the trace ID.",
                    "internal_server_error")
                .ExecuteAsync(context);
        }
    }
}