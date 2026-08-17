using System.Text.Json;
using Library.Domain.Exceptions;
using Library.Api.Contracts.Common;

namespace Library.Api.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (NotFoundException ex)
        {
            await WriteError(
                context,
                StatusCodes.Status404NotFound,
                ex.Message);
        }
        catch (ConflictException ex)
        {
            await WriteError(
                context,
                StatusCodes.Status409Conflict,
                ex.Message);
        }
        catch (BusinessRuleException ex)
        {
            await WriteError(
                context,
                StatusCodes.Status400BadRequest,
                ex.Message);
        }
        catch (Exception ex)
        {
            await WriteError(
                context,
                StatusCodes.Status500InternalServerError,
                ex.Message);
        }
    }

    private static async Task WriteError(
        HttpContext context,
        int statusCode,
        string message)
    {
        context.Response.ContentType =
            "application/json";

        context.Response.StatusCode =
            statusCode;

        var response =
            new ErrorResponse(
                statusCode,
                message,
                context.TraceIdentifier);

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(response));
    }
}