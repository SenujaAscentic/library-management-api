using MediatR;
using Library.Application.Features.Books;

namespace Library.Application.Features.Books.Commands.CreateBook;

public record CreateBookCommand(
    string Title,
    string Author,
    string Isbn,
    int PublishedYear,
    int TotalCopies) : IRequest<BookResponse>;