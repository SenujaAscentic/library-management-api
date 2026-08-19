using Library.Application.Abstractions.Messaging;

namespace Library.Application.Features.Books.Queries.GetBookById;

    public record GetBookByIdQuery(Guid Id) : IQuery<BookResponse>;
    

