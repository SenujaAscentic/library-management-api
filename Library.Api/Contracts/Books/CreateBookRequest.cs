namespace Library.Api.Contracts.Books;

public record CreateBookRequest(
    string Title,
    string Author,
    string Isbn,
    int PublishedYear,
    int TotalCopies);