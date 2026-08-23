using FluentValidation;
using Library.Api.Common.Validation;
using Library.Application.Features.Members.Commands.CreateMember;
using Library.Application.Features.Members.Commands.DeleteMember;
using Library.Application.Features.Members.Commands.UpdateMember;
using Library.Application.Features.Members.Queries.GetAllMembers;
using Library.Application.Features.Members.Queries.GetMemberById;
using MediatR;

namespace Library.Api.Endpoints;

public static class MemberEndpoints
{
    public static void MapMemberEndpoints(this WebApplication app)
    {
        app.MapPost("/api/members", async (CreateMemberCommand command, IValidator<CreateMemberCommand> validator, IMediator mediator, HttpContext httpContext) =>
        {
            var result = await ValidationHelper.ValidateAsync(command, validator, httpContext);
            if (result is not null) return result;
            var member = await mediator.Send(command);
            return Results.Created($"/api/members/{member.Id}", member);
        });

        app.MapGet("/api/members", async (IMediator mediator) =>
        {
            var members = await mediator.Send(new GetAllMembersQuery());
            return Results.Ok(members);
        });

        app.MapGet("/api/members/{id:guid}", async (Guid id, IMediator mediator) =>
        {
            var member = await mediator.Send(new GetMemberByIdQuery(id));
            return Results.Ok(member);
        });

        app.MapPut("/api/members/{id:guid}", async (Guid id, UpdateMemberRequest body, IValidator<UpdateMemberCommand> validator, IMediator mediator, HttpContext httpContext) =>
        {
            var command = new UpdateMemberCommand(id, body.FullName, body.Email, body.PhoneNumber);
            var result = await ValidationHelper.ValidateAsync(command, validator, httpContext);
            if (result is not null) return result;
            var member = await mediator.Send(command);
            return Results.Ok(member);
        });

        app.MapDelete("/api/members/{id:guid}", async (Guid id, IMediator mediator, HttpContext httpContex) =>
        {
            await mediator.Send(new DeleteMemberCommand(id));
            return Results.NoContent();
        });
    }
}