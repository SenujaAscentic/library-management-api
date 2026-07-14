using Library.Api.Contracts.Borrowings;
using Library.Api.Domain.Entities;
using Library.Api.Domain.Enums;
using Library.Api.Infrastructure.Repositories.Interfaces;

public class BorrowingService : IBorrowingService
{
    private readonly IBookRepository _bookRepository;
    private readonly IMemberRepository _memberRepository;
    private readonly IBorrowingRepository _borrowingRepository;

    public BorrowingService(
        IBookRepository bookRepository,
        IMemberRepository memberRepository,
        IBorrowingRepository borrowingRepository)
    {
        _bookRepository = bookRepository;
        _memberRepository = memberRepository;
        _borrowingRepository = borrowingRepository;
    }
    public async Task<BorrowingResponse> BorrowAsync(
    BorrowBookRequest request)
{
    var book =
        await _bookRepository
            .GetByIdAsync(request.BookId);

    if (book is null)
    {
        throw new Exception("Book not found");
    }

    var member =
        await _memberRepository
            .GetByIdAsync(request.MemberId);

    if (member is null)
    {
        throw new Exception("Member not found");
    }

    if (!member.IsActive)
    {
        throw new Exception(
            "Member is inactive");
    }

    if (book.AvailableCopies <= 0)
    {
        throw new Exception(
            "Book is unavailable");
    }

    var activeBorrowings =
        await _borrowingRepository
            .GetActiveBorrowingsByMemberAsync(
                request.MemberId);

    if (activeBorrowings.Count >= 3)
    {
        throw new Exception(
            "Member borrowing limit exceeded");
    }

    var borrowing = new Borrowing
    {
        Id = Guid.NewGuid(),
        BookId = book.Id,
        MemberId = member.Id,
        BorrowedDate = DateTime.UtcNow,
        DueDate = DateTime.UtcNow.AddDays(14),
        ReturnedDate = null,
        Status = BorrowingStatus.Borrowed
    };

    book.AvailableCopies--;

    await _borrowingRepository
        .AddAsync(borrowing);

    _bookRepository.Update(book);

    await _borrowingRepository
        .SaveChangesAsync();

    return new BorrowingResponse(
        borrowing.Id,
        borrowing.BookId,
        borrowing.MemberId,
        borrowing.BorrowedDate,
        borrowing.DueDate,
        borrowing.ReturnedDate,
        borrowing.Status.ToString());
}
public async Task<List<BorrowingResponse>>
    GetAllAsync()
{
    var borrowings =
        await _borrowingRepository
            .GetAllAsync();

    return borrowings
        .Select(x =>
            new BorrowingResponse(
                x.Id,
                x.BookId,
                x.MemberId,
                x.BorrowedDate,
                x.DueDate,
                x.ReturnedDate,
                x.Status.ToString()))
        .ToList();
}
public async Task<List<BorrowingResponse>>
    GetByMemberAsync(Guid memberId)
{
    var borrowings =
        await _borrowingRepository
            .GetByMemberIdAsync(memberId);

    return borrowings
        .Select(x =>
            new BorrowingResponse(
                x.Id,
                x.BookId,
                x.MemberId,
                x.BorrowedDate,
                x.DueDate,
                x.ReturnedDate,
                x.Status.ToString()))
        .ToList();
}
public async Task ReturnAsync(Guid borrowingId)
{
    var borrowing =
        await _borrowingRepository
            .GetByIdAsync(borrowingId);

    if (borrowing is null)
    {
        throw new Exception(
            "Borrowing record not found");
    }

    if (borrowing.ReturnedDate is not null)
    {
        throw new Exception(
            "Book already returned");
    }

    var book =
        await _bookRepository
            .GetByIdAsync(borrowing.BookId);

    if (book is null)
    {
        throw new Exception(
            "Book not found");
    }

    borrowing.ReturnedDate =
        DateTime.UtcNow;

    borrowing.Status =
        BorrowingStatus.Returned;

    book.AvailableCopies++;

    _borrowingRepository.Update(borrowing);

    _bookRepository.Update(book);

    await _borrowingRepository
        .SaveChangesAsync();
}
}