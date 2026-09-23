using Library.Api.Common.Authorization;
using Library.Api.Common.ErrorHandling;
using Library.Application.Features.Borrowings.Commands.BorrowBook;
using Library.Application.Features.Borrowings.Commands.ReturnBook;
using Library.Application.Features.Borrowings.Queries.GetAllBorrowings;
using Library.Application.Features.Borrowings.Queries.GetBorrowingsByMember;
using Library.Domain.Exceptions;
using MediatR;

namespace Library.Api.Endpoints;

public static class BorrowingEndpoints
{
    public static void MapBorrowingEndpoints(this WebApplication app)
    {
        app.MapPost("/api/borrowings", async (BorrowBookCommand command, IMediator mediator, HttpContext httpContext) =>
        {
            if (!AuthorizationHelper.IsOwnerOrAdmin(httpContext.User, command.MemberId))
                throw new ForbiddenException("forbidden", "You can only borrow books for yourself.");

            var result = await mediator.Send(command);
            return result.IsSuccess
                ? Results.Created($"/api/borrowings/{result.Value.Id}", result.Value)
                : result.Error.ToProblemDetails(httpContext);
        }).RequireAuthorization();

        app.MapGet("/api/borrowings", async (IMediator mediator, HttpContext httpContext) =>
        {
            if (!AuthorizationHelper.IsAdmin(httpContext.User))
                throw new ForbiddenException("forbidden", "Only Admins can view all borrowings.");

            var borrowings = await mediator.Send(new GetAllBorrowingsQuery());
            return Results.Ok(borrowings);
        }).RequireAuthorization();

        app.MapGet("/api/members/{memberId:guid}/borrowings", async (Guid memberId, IMediator mediator, HttpContext httpContext) =>
        {
            if (!AuthorizationHelper.IsOwnerOrAdmin(httpContext.User, memberId))
                throw new ForbiddenException("forbidden", "You can only view your own borrowing history.");

            var borrowings = await mediator.Send(new GetBorrowingsByMemberQuery(memberId));
            return Results.Ok(borrowings);
        }).RequireAuthorization();

        app.MapPost("/api/borrowings/{id:guid}/return", async (Guid id, IMediator mediator, HttpContext httpContext) =>
        {
            Guid? requestingMemberId = null;
            if (!AuthorizationHelper.IsAdmin(httpContext.User))
            {
                requestingMemberId = AuthorizationHelper.GetMemberId(httpContext.User)
                    ?? throw new ForbiddenException("forbidden", "Unable to determine member identity.");
            }

            var result = await mediator.Send(new ReturnBookCommand(id, requestingMemberId));
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : result.Error.ToProblemDetails(httpContext);
        }).RequireAuthorization();
    }
}