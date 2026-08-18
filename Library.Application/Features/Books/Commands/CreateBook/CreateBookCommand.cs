using MediatR;
using Library.Application.Features.Books;
using Library.Application.Abstractions.Messaging;

namespace Library.Application.Features.Books.Commands.CreateBook;

public record CreateBookCommand(
    string Title,
    string Author,
    string Isbn,
    int PublishedYear,
    int TotalCopies) : ICommand<BookResponse>;