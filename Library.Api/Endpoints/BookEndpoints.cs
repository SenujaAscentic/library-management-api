
using FluentValidation;
using Library.Api.Application.Interfaces;
using Library.Api.Common.Validation;
using Library.Api.Contracts.Books;
using Library.Api.Contracts.Common;
using Library.Application.Features.Books.Commands.CreateBook;
using Library.Application.Features.Books.Queries.GetBookById;
using MediatR;

namespace Library.Api.Endpoints;

public static class BookEndpoints
{
    public static void MapBookEndpoints(this WebApplication app)
    {
        app.MapPost("/api/books", async (CreateBookCommand command, IValidator<CreateBookCommand> validator, IMediator mediator) =>
        {
            var result = await ValidationHelper.ValidateAsync(command, validator);
            if (result is not null)
            {
                return result;
            }
            var book = await mediator.Send(command);
            return Results.Created($"/api/books/{book.Id}", book);
        });
        app.MapGet("/api/books", async(IBookService service) =>
        {
            var books = await service.GetAllAsync();
            return Results.Ok(books);
        });
        app.MapGet("/api/books/{id:guid}", async(Guid id, IMediator mediator) =>
        {
            var query = new GetBookByIdQuery(id);
            var book = await mediator.Send(query);
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