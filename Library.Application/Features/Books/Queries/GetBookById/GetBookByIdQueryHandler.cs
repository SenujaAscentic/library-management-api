using Library.Application.Abstractions.Messaging;
using Library.Application.Abstractions.Repositories;
using Library.Domain.Exceptions;

namespace Library.Application.Features.Books.Queries.GetBookById
{
    public class GetBookByIdQueryHandler(IBookRepository bookRepository)
        : IQueryHandler<GetBookByIdQuery, BookResponse>
    {
        public async Task<BookResponse> Handle(GetBookByIdQuery request, CancellationToken cancellationToken)
        {
            var book = await bookRepository.GetByIdAsync(request.Id);
            if (book is null)
            {
                throw new NotFoundException("book_not_found","Book not found.");
            }
            return new BookResponse(
                book.Id,
                book.Title,
                book.Author,
                book.Isbn,
                book.PublishedYear,
                book.TotalCopies,
                book.AvailableCopies);
        }
    }
}
