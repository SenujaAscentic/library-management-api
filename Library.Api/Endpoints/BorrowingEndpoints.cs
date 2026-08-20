using Library.Api.Common.ErrorHandling;
using Library.Application.Features.Borrowings.Commands.BorrowBook;
using Library.Application.Features.Borrowings.Commands.ReturnBook;
using Library.Application.Features.Borrowings.Queries.GetAllBorrowings;
using Library.Application.Features.Borrowings.Queries.GetBorrowingsByMember;
using MediatR;

namespace Library.Api.Endpoints;

public static class BorrowingEndpoints
{
    public static void MapBorrowingEndpoints(this WebApplication app)
    {
        app.MapPost("/api/borrowings", async (BorrowBookCommand command , IMediator mediator)=>
        {
            var result = await mediator.Send(command);
            return result.IsSuccess
            ? Results.Created($"/api/borrowings/{result.Value.Id}", result.Value)
            : result.Error.ToProblemDetails();

        });
        app.MapGet("/api/borrowings", async(IMediator mediator) =>
        {
            var borrowings = await mediator.Send(new GetAllBorrowingsQuery());

            return Results.Ok(borrowings);
        });
        app.MapGet("/api/members/{memberId:guid}/borrowings", async (Guid memberId, IMediator mediator) =>
        {
            var borrowings = await mediator.Send(new GetBorrowingsByMemberQuery(memberId));
            return Results.Ok(borrowings);
        });
        app.MapPost("/api/borrowings/{id:guid}/return", async(Guid id, IMediator mediator) =>
        {
            var result = await mediator.Send(new ReturnBookCommand(id));
            return result.IsSuccess
            ? Results.Ok(result.Value)
            : result.Error.ToProblemDetails();
        });

    }
}