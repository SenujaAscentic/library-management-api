using Library.Application.Abstractions;
using Library.Application.Abstractions.Messaging;
using Library.Application.Abstractions.Repositories;
using Library.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Library.Application.Features.Books.Commands.DeleteBook;

    public class DeleteBookCommandHandler(IBookRepository bookRepository ,IBorrowingRepository borrowingRepository,IUnitOfWork unitOfWork, ILogger<DeleteBookCommandHandler> logger)
        :ICommandHandler<DeleteBookCommand>
    {
        public async Task Handle(DeleteBookCommand command , CancellationToken cancellationToken)
        {
            var book = await bookRepository.GetByIdAsync(command.Id);
            if (book is null)
            {
                logger.LogWarning("Book with ID {BookId} not found.", command.Id);
                throw new NotFoundException("book_not_found","Book not found.");
            }
        var hasActiveBorrowings = await borrowingRepository.HasActiveBorrowingForBookAsync(command.Id);
        if (hasActiveBorrowings)
        {
            logger.LogWarning("Delete rejected: book {BookId} has active borrowings.", command.Id);
            throw new BusinessRuleException("book_has_active_borrowings","Cannot delete a book that has active borrowings.");
        }
        bookRepository.Delete(book);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Book with ID {BookId} deleted successfully.", book.Id);
        }
    }

