 namespace Library.Application.Features.Borrowings;

public record BorrowingResponse(
    Guid Id,
    Guid BookId,
    Guid MemberId,
    DateTime BorrowedDate,
    DateTime DueDate,
    DateTime? ReturnedDate,
    string Status);