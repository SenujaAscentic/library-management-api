using Library.Application.Abstractions.Messaging;
using Library.Application.Abstractions.Repositories;
using Library.Application.Abstractions.Results;
using Library.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Library.Application.Features.Borrowings.Commands.ReturnBook;

public class ReturnBookCommandHandler(
    IBorrowingRepository borrowingRepository,
    IBookRepository bookRepository,
    ILogger<ReturnBookCommandHandler> logger)
    : ICommandHandler<ReturnBookCommand, Result<BorrowingResponse>>
{
    public async Task<Result<BorrowingResponse>> Handle(ReturnBookCommand request, CancellationToken cancellationToken)
    {
        var borrowing = await borrowingRepository.GetByIdAsync(request.BorrowingId);
        if (borrowing is null)
        {
            logger.LogWarning("Return rejected: borrowing {BorrowingId} not found", request.BorrowingId);
            return Result<BorrowingResponse>.Failure(BorrowingErrors.BorrowingNotFound);
        }

        if (borrowing.ReturnedDate is not null)
        {
            logger.LogWarning("Return rejected: borrowing {BorrowingId} already returned", request.BorrowingId);
            return Result<BorrowingResponse>.Failure(BorrowingErrors.AlreadyReturned);
        }

        var book = await bookRepository.GetByIdAsync(borrowing.BookId);
        if (book is null)
        {
            logger.LogWarning("Return rejected: book {BookId} not found for borrowing {BorrowingId}", borrowing.BookId, request.BorrowingId);
            return Result<BorrowingResponse>.Failure(BorrowingErrors.BookNotFound);
        }

        borrowing.ReturnedDate = DateTime.UtcNow;
        borrowing.Status = BorrowingStatus.Returned;
        book.AvailableCopies++;

        borrowingRepository.Update(borrowing);
        bookRepository.Update(book);
        await borrowingRepository.SaveChangesAsync();

        logger.LogInformation("Borrowing {BorrowingId} returned, book {BookId} now has {AvailableCopies} available",
            borrowing.Id, book.Id, book.AvailableCopies);

        var response = new BorrowingResponse(borrowing.Id, borrowing.BookId, borrowing.MemberId, borrowing.BorrowedDate, borrowing.DueDate, borrowing.ReturnedDate, borrowing.Status.ToString());
        return Result<BorrowingResponse>.Success(response);
    }
}