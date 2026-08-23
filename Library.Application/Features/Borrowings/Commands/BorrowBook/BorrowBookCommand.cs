using Library.Application.Abstractions.Messaging;
using Library.Application.Abstractions.Results;

namespace Library.Application.Features.Borrowings.Commands.BorrowBook;

public record BorrowBookCommand(Guid MemberId, Guid BookId) : ICommand<Result<BorrowingResponse>>;