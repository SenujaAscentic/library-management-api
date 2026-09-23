using FluentValidation;
using Library.Api.Common.Authorization;
using Library.Api.Common.Validation;
using Library.Application.Features.Members.Commands.CreateMember;
using Library.Application.Features.Members.Commands.DeleteMember;
using Library.Application.Features.Members.Commands.UpdateMember;
using Library.Application.Features.Members.Queries.GetAllMembers;
using Library.Application.Features.Members.Queries.GetMemberById;
using Library.Domain.Exceptions;
using MediatR;

namespace Library.Api.Endpoints;

public static class MemberEndpoints
{
    public static void MapMemberEndpoints(this WebApplication app)
    {
        app.MapPost("/api/members", async (CreateMemberCommand command, IValidator<CreateMemberCommand> validator, IMediator mediator, HttpContext httpContext) =>
        {
            if (!AuthorizationHelper.IsAdmin(httpContext.User))
                throw new ForbiddenException("forbidden", "Only Admins can create members.");

            var result = await ValidationHelper.ValidateAsync(command, validator, httpContext);
            if (result is not null) return result;
            var member = await mediator.Send(command);
            return Results.Created($"/api/members/{member.Id}", member);
        }).RequireAuthorization();

        app.MapGet("/api/members", async (IMediator mediator, HttpContext httpContext) =>
        {
            if (!AuthorizationHelper.IsAdmin(httpContext.User))
                throw new ForbiddenException("forbidden", "Only Admins can view the full member list.");

            var members = await mediator.Send(new GetAllMembersQuery());
            return Results.Ok(members);
        }).RequireAuthorization();

        app.MapGet("/api/members/{id:guid}", async (Guid id, IMediator mediator, HttpContext httpContext) =>
        {
            if (!AuthorizationHelper.IsOwnerOrAdmin(httpContext.User, id))
                throw new ForbiddenException("forbidden", "You can only view your own member record.");

            var member = await mediator.Send(new GetMemberByIdQuery(id));
            return Results.Ok(member);
        }).RequireAuthorization();

        app.MapPut("/api/members/{id:guid}", async (Guid id, UpdateMemberRequest body, IValidator<UpdateMemberCommand> validator, IMediator mediator, HttpContext httpContext) =>
        {
            if (!AuthorizationHelper.IsOwnerOrAdmin(httpContext.User, id))
                throw new ForbiddenException("forbidden", "You can only update your own member record.");

            var command = new UpdateMemberCommand(id, body.FullName, body.Email, body.PhoneNumber);
            var result = await ValidationHelper.ValidateAsync(command, validator, httpContext);
            if (result is not null) return result;
            var member = await mediator.Send(command);
            return Results.Ok(member);
        }).RequireAuthorization();

        app.MapDelete("/api/members/{id:guid}", async (Guid id, IMediator mediator, HttpContext httpContext) =>
        {
            if (!AuthorizationHelper.IsAdmin(httpContext.User))
                throw new ForbiddenException("forbidden", "Only Admins can delete members.");

            await mediator.Send(new DeleteMemberCommand(id));
            return Results.NoContent();
        }).RequireAuthorization();
    }
}
