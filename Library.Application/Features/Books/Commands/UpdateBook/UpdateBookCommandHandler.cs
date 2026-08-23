using Library.Application.Abstractions;
using Library.Application.Abstractions.Messaging;
using Library.Application.Abstractions.Repositories;
using Library.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Library.Application.Features.Books.Commands.UpdateBook
{
    public class UpdateBookCommandHandler(IBookRepository bookRepository,IUnitOfWork unitOfWork, ILogger<UpdateBookCommandHandler> logger)
        : ICommandHandler<UpdateBookCommand, BookResponse>
    {
        public async Task<BookResponse> Handle(UpdateBookCommand command, CancellationToken cancellationToken)
        {
            var book = await bookRepository.GetByIdAsync(command.Id);
            if (book is null)
            {
                logger.LogWarning("Book with ID {BookId} not found.", command.Id);
                throw new NotFoundException("book_not_found","Book not found.");
            }
            var duplicateBook = await bookRepository.GetByIsbnAsync(command.Isbn);
            if (duplicateBook is not null && duplicateBook.Id != command.Id)
            {
                logger.LogWarning("Book with ISBN {Isbn} already exists.", command.Isbn);
                throw new ConflictException("duplicate_isbn","ISBN already exists.");
            }

            book.UpdateDetails(command.Title, command.Author, command.Isbn, command.PublishedYear);

            try
            {
                book.UpdateTotalCopies(command.TotalCopies);
            }
            catch (BusinessRuleException ex)
            {
                logger.LogWarning(
                    "Update rejected: cannot change total copies for book {BookId} to {NewTotal} — {Reason}",
                    command.Id, command.TotalCopies, ex.Message);
                throw;
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);


            logger.LogInformation("Book with ID {BookId} updated successfully.", command.Id);

            return new BookResponse(book.Id, book.Title, book.Author, book.Isbn, book.PublishedYear, book.TotalCopies, book.AvailableCopies);
        }
    }
}
