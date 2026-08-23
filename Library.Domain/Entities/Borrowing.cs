namespace Library.Domain.Entities;

using Library.Domain.Enums;
using Library.Domain.Exceptions;

public sealed class Borrowing : BaseEntity
{
    public Guid MemberId { get; private set; }
    public Member Member { get; private set; } = null!;
    public Guid BookId { get; private set; }
    public Book Book { get; private set; } = null!;
    public DateTime BorrowedDate { get; private set; }
    public DateTime DueDate { get; private set; }
    public DateTime? ReturnedDate { get; private set; }
    public BorrowingStatus Status { get; private set; }

    private Borrowing() { } // EF Core

    public static Borrowing Create(Guid memberId, Guid bookId)
    {
        var borrowedDate = DateTime.UtcNow;
        return new Borrowing
        {
            Id = Guid.NewGuid(),
            MemberId = memberId,
            BookId = bookId,
            BorrowedDate = borrowedDate,
            DueDate = borrowedDate.AddDays(14),
            Status = BorrowingStatus.Borrowed
        };
    }

    public void MarkAsReturned()
    {
        

        ReturnedDate = DateTime.UtcNow;
        Status = BorrowingStatus.Returned;
    }
}