using Library.Api.Application.Interfaces;
using Library.Api.Contracts.Borrowings;

namespace Library.Api.Endpoints;

public static class BorrowingEndpoints
{
    public static void MapBorrowingEndpoints(this WebApplication app)
    {
        app.MapPost("/api/borrowings", async (BorrowBookRequest request, IBorrowingService service)=>
        {
            var borrowing = await service.BorrowAsync(request);
            return Results.Created($"/api/borrowings/{borrowing.Id}", borrowing);
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