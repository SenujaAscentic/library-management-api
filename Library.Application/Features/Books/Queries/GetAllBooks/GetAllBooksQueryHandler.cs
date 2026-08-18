using Library.Application.Abstractions.Messaging;
using Library.Application.Abstractions.Repositories;

namespace Library.Application.Features.Books.Queries.GetAllBooks
{
    public class GetAllBooksQueryHandler(IBookRepository bookRepository)
        : IQueryHandler<GetAllBooksQuery, List<BookResponse>>
    {
        public async Task<List<BookResponse>> Handle(GetAllBooksQuery query, CancellationToken cancellationToken)
        {
            var books = await bookRepository.GetAllAsync();
            return books.Select(book => new BookResponse(
                book.Id,
                book.Title,
                book.Author,
                book.Isbn,
                book.PublishedYear,
                book.TotalCopies,
                book.AvailableCopies)).ToList();
        }
    }
}
