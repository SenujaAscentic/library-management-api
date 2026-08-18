using Library.Application.Abstractions.Messaging;
using Library.Application.Features.Books;

namespace Library.Application.Features.Books.Queries.GetAllBooks;

    public record GetAllBooksQuery : IQuery<List<BookResponse>>;
    

