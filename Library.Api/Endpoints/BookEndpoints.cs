using FluentValidation;
using Library.Api.Common.Authorization;
using Library.Api.Common.Validation;
using Library.Application.Features.Books.Commands.CreateBook;
using Library.Application.Features.Books.Commands.DeleteBook;
using Library.Application.Features.Books.Commands.UpdateBook;
using Library.Application.Features.Books.Queries.GetAllBooks;
using Library.Application.Features.Books.Queries.GetBookById;
using Library.Domain.Exceptions;
using MediatR;

namespace Library.Api.Endpoints;

public static class BookEndpoints
{
    public static void MapBookEndpoints(this WebApplication app)
    {
        app.MapPost("/api/books", async (CreateBookCommand command, IValidator<CreateBookCommand> validator, IMediator mediator, HttpContext httpContext) =>
        {
            if (!AuthorizationHelper.IsAdmin(httpContext.User))
                throw new ForbiddenException("forbidden", "Only Admins can create books.");

            var result = await ValidationHelper.ValidateAsync(command, validator, httpContext);
            if (result is not null) return result;
            var book = await mediator.Send(command);
            return Results.Created($"/api/books/{book.Id}", book);
        }).RequireAuthorization();

        app.MapGet("/api/books", async (IMediator mediator) =>
        {
            var books = await mediator.Send(new GetAllBooksQuery());
            return Results.Ok(books);
        }).RequireAuthorization();

        app.MapGet("/api/books/{id:guid}", async (Guid id, IMediator mediator) =>
        {
            var book = await mediator.Send(new GetBookByIdQuery(id));
            return Results.Ok(book);
        }).RequireAuthorization();

        app.MapPut("/api/books/{id:guid}", async (Guid id, UpdateBookRequest body, IMediator mediator, IValidator<UpdateBookCommand> validator, HttpContext httpContext) =>
        {
            if (!AuthorizationHelper.IsAdmin(httpContext.User))
                throw new ForbiddenException("forbidden", "Only Admins can update books.");

            var command = new UpdateBookCommand(id, body.Title, body.Author, body.Isbn, body.PublishedYear, body.TotalCopies);
            var result = await ValidationHelper.ValidateAsync(command, validator, httpContext);
            if (result is not null) return result;
            var book = await mediator.Send(command);
            return Results.Ok(book);
        }).RequireAuthorization();

        app.MapDelete("/api/books/{id:guid}", async (Guid id, IMediator mediator, HttpContext httpContext) =>
        {
            if (!AuthorizationHelper.IsAdmin(httpContext.User))
                throw new ForbiddenException("forbidden", "Only Admins can delete books.");

            await mediator.Send(new DeleteBookCommand(id));
            return Results.NoContent();
        }).RequireAuthorization();
    }
}