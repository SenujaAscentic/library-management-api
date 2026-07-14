namespace Library.Api.Contracts.Books;

public record UpdateBookRequest(
    string Title,
    string Author,
    string Isbn,
    int PublishedYear,
    int TotalCopies);