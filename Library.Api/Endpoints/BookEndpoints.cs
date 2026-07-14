
using Library.Api.Application.Interfaces;
using Library.Api.Contracts.Books;

namespace Library.Api.Endpoints;

public static class BookEndpoints
{
    public static void MapBookEndpoints(this WebApplication app)
    {
        app.MapPost("/api/books", async(CreateBookRequest request , IBookService service) =>
        {
            var book = await service.CreateAsync(request);
            return Results.Created($"/api/books/{book.Id}", book);
        });
        app.MapGet("/api/books", async(IBookService service) =>
        {
            var books = await service.GetAllAsync();
            return Results.Ok(books);
        });
        app.MapGet("/api/books/{id:guid}", async(Guid id, IBookService service) =>
        {
            var book = await service.GetByIdAsync(id);
            if(book is null)
            {
                return Results.NotFound();
            }
            else
            {
                return Results.Ok(book);
            }
        });
        app.MapPut("/api/books/{id:guid}", async(Guid id, UpdateBookRequest request, IBookService service) =>
        { await service.UpdateAsync(id, request);
            return Results.NoContent();
        });
        app.MapDelete("/api/books/{id:guid}", async(Guid id, IBookService service) =>
        {
            await service.DeleteAsync(id);
            return Results.NoContent();
        });
    }
}