using Library.Application.Abstractions.Messaging;

namespace Library.Application.Features.Books.Commands.UpdateBook;

public record UpdateBookCommand(
Guid Id,
string Title,
string Author,
string Isbn,
int PublishedYear,
int TotalCopies) : ICommand<BookResponse>;

