namespace Library.Application.Features.Books;

public record BookResponse(
    Guid Id,
    string Title,
    string Author,
    string Isbn,
    int PublishedYear,
    int TotalCopies,
    int AvailableCopies);