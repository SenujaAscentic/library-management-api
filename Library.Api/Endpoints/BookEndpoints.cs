
using FluentValidation;
using Library.Api.Application.Interfaces;
using Library.Api.Common.Validation;
using Library.Api.Contracts.Books;
using Library.Api.Contracts.Common;

namespace Library.Api.Endpoints;

public static class BookEndpoints
{
    public static void MapBookEndpoints(this WebApplication app)
    {
        app.MapPost("/api/books", async(CreateBookRequest request ,IValidator<CreateBookRequest> validator, IBookService service) =>
        {
            var result = await ValidationHelper.ValidateAsync(request, validator);
            if (result is not null)
            {
                return result;
            }
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
            return Results.Ok(book);
           
        });
        app.MapPut("/api/books/{id:guid}", async(Guid id, UpdateBookRequest request,IValidator<UpdateBookRequest> validator, IBookService service) =>
        {
            var result = await ValidationHelper.ValidateAsync(request, validator);
            if (result is not null)
            {
                return result;
            }
            await service.UpdateAsync(id, request);
            return Results.NoContent();
        });
        app.MapDelete("/api/books/{id:guid}", async(Guid id, IBookService service) =>
        {
            await service.DeleteAsync(id);
            return Results.NoContent();
        });
    }
}