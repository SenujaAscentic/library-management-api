using Library.Application.Abstractions.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Application.Features.Borrowings;
public static class BorrowingErrors
{
    public static Error BookNotFound => new("book_not_found", "Book not found.", ErrorType.NotFound);
    public static Error MemberNotFound => new("member_not_found", "Member not found.", ErrorType.NotFound);
    public static Error MemberInactive => new("member_inactive", "Member is inactive.", ErrorType.BusinessRule);
    public static Error BookUnavailable => new("book_unavailable", "Book is unavailable.", ErrorType.BusinessRule);
    public static Error BorrowingLimitExceeded => new("borrowing_limit_exceeded", "Member borrowing limit exceeded.", ErrorType.BusinessRule);
    public static Error BorrowingNotFound => new("borrowing_not_found", "Borrowing record not found.", ErrorType.NotFound);
    public static Error AlreadyReturned => new("book_already_returned", "Book already returned.", ErrorType.BusinessRule);
}
