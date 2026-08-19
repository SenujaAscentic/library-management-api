using Library.Application.Abstractions.Messaging;
using Library.Application.Abstractions.Repositories;
using Library.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Library.Application.Features.Books.Commands.UpdateBook
{
    public class UpdateBookCommandHandler(IBookRepository bookRepository, ILogger<UpdateBookCommandHandler> logger)
        : ICommandHandler<UpdateBookCommand, BookResponse>
    {
        public async Task<BookResponse> Handle(UpdateBookCommand command, CancellationToken cancellationToken)
        {
            var book = await bookRepository.GetByIdAsync(command.Id);
            if (book is null)
            {
                logger.LogWarning("Book with ID {BookId} not found.", command.Id);
                throw new NotFoundException("Book not found.");
            }
            var duplicateBook = await bookRepository.GetByIsbnAsync(command.Isbn);
            if (duplicateBook is not null && duplicateBook.Id != command.Id)
            {
                logger.LogWarning("Book with ISBN {Isbn} already exists.", command.Isbn);
                throw new ConflictException("ISBN already exists.");
            }

            var borrowedCount = book.TotalCopies - book.AvailableCopies;
            if (command.TotalCopies < borrowedCount)
            {
                logger.LogWarning(
                    "Update rejected: cannot reduce total copies for book {BookId} to {NewTotal}, {BorrowedCount} currently borrowed",
                    command.Id, command.TotalCopies, borrowedCount);
                throw new BusinessRuleException(
                    $"Cannot reduce total copies below the number currently borrowed ({borrowedCount}).");
            }


            book.Title = command.Title;
            book.Author = command.Author;
            book.Isbn = command.Isbn;
            book.PublishedYear = command.PublishedYear;
            book.TotalCopies = command.TotalCopies;
            book.AvailableCopies = command.TotalCopies - borrowedCount;
            await bookRepository.SaveChangesAsync();

            logger.LogInformation("Book with ID {BookId} updated successfully.", command.Id);

            return new BookResponse(book.Id, book.Title, book.Author, book.Isbn, book.PublishedYear, book.TotalCopies, book.AvailableCopies);
        }
    }
}
