namespace Library.Application.Features.Books.Commands.UpdateBook;

public record UpdateBookRequest(
    string Title,
    string Author,
    string Isbn,
    int PublishedYear,
    int TotalCopies);
