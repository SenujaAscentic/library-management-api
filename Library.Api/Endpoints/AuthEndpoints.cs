
using FluentValidation;
using Library.Api.Common.Validation;
using Library.Application.Features.Auth.Register;
using MediatR;

namespace Library.Api.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        app.MapPost("/api/auth/register", async (RegisterCommand command, IValidator<RegisterCommand> validator, IMediator mediator, HttpContext httpContext) =>
        {
            var result = await ValidationHelper.ValidateAsync(command, validator, httpContext);
            if (result is not null)
            {
                return result;
            }
            var memberId = await mediator.Send(command);
            return Results.Created($"/api/members/{memberId}", new { memberId });
        });
    }
}