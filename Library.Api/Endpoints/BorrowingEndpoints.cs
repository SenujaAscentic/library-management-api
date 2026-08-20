using Library.Api.Application.Interfaces;
using Library.Api.Common.ErrorHandling;
using Library.Application.Features.Borrowings.Commands;
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
        app.MapGet("/api/borrowings", async(IBorrowingService service) =>
        {
           
            return Results.Ok(await service.GetAllAsync());
        });
        app.MapGet("/api/borrowings/{memberId:guid}", async(Guid memberId, IBorrowingService service) =>
        {
            return Results.Ok(await service.GetByMemberAsync(memberId));
        });
        app.MapPost("/api/borrowings/{id:guid}/return", async(Guid id, IBorrowingService service) =>
        {
            await service.ReturnAsync(id);
            return Results.NoContent();
        });

    }
}