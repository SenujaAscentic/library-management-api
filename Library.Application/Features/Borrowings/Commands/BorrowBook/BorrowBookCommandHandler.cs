using Library.Application.Abstractions;
using Library.Application.Abstractions.Messaging;
using Library.Application.Abstractions.Repositories;
using Library.Application.Abstractions.Results;
using Library.Domain.Entities;
using Library.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Library.Application.Features.Borrowings.Commands.BorrowBook;

public class BorrowBookCommandHandler(
    IBookRepository bookRepository,
    IMemberRepository memberRepository,
    IBorrowingRepository borrowingRepository,
    IUnitOfWork unitOfWork,
    ILogger<BorrowBookCommandHandler> logger)
    : ICommandHandler<BorrowBookCommand, Result<BorrowingResponse>>
{
    public async Task<Result<BorrowingResponse>> Handle(BorrowBookCommand request, CancellationToken cancellationToken)
    {
        var book = await bookRepository.GetByIdAsync(request.BookId);
        if (book is null)
        {
            logger.LogWarning("Borrow rejected: book {BookId} not found", request.BookId);
            return Result<BorrowingResponse>.Failure(BorrowingErrors.BookNotFound);
        }

        var member = await memberRepository.GetByIdAsync(request.MemberId);
        if (member is null)
        {
            logger.LogWarning("Borrow rejected: member {MemberId} not found", request.MemberId);
            return Result<BorrowingResponse>.Failure(BorrowingErrors.MemberNotFound);
        }

        if (!member.IsActive)
        {
            logger.LogWarning("Borrow rejected: member {MemberId} is inactive", request.MemberId);
            return Result<BorrowingResponse>.Failure(BorrowingErrors.MemberInactive);
        }

        if (book.AvailableCopies <= 0)
        {
            logger.LogWarning("Borrow rejected: book {BookId} has no available copies", request.BookId);
            return Result<BorrowingResponse>.Failure(BorrowingErrors.BookUnavailable);
        }

        var activeBorrowings = await borrowingRepository.GetActiveBorrowingsByMemberAsync(request.MemberId);
        if (activeBorrowings.Count >= 3)
        {
            logger.LogWarning("Borrow rejected: member {MemberId} has reached the borrowing limit", request.MemberId);
            return Result<BorrowingResponse>.Failure(BorrowingErrors.BorrowingLimitExceeded);
        }

        var borrowing = Borrowing.Create(member.Id, book.Id);

        book.DecrementAvailableCopies();

        await borrowingRepository.AddAsync(borrowing);
        bookRepository.Update(book);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Book {BookId} borrowed by member {MemberId}, due {DueDate}", book.Id, member.Id, borrowing.DueDate);

        var response = new BorrowingResponse(borrowing.Id, borrowing.BookId, borrowing.MemberId, borrowing.BorrowedDate, borrowing.DueDate, borrowing.ReturnedDate, borrowing.Status.ToString());
        return Result<BorrowingResponse>.Success(response);
    }
}