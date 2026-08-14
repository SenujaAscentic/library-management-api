using Library.Domain.Enums;

namespace Library.Domain.Entities;

public class Borrowing
{
    public Guid Id { get; set; }

    public Guid MemberId { get; set; }

    public Member Member { get; set; } = null!;

    public Guid BookId { get; set; }

    public Book Book { get; set; } = null!;

    public DateTime BorrowedDate { get; set; }

    public DateTime DueDate { get; set; }

    public DateTime? ReturnedDate { get; set; }

    public BorrowingStatus Status { get; set; }
}