namespace Library.Api.Contracts.Borrowings;

public record BorrowBookRequest(
    Guid MemberId,
    Guid BookId);