using Library.Application.Abstractions.Messaging;
using Library.Application.Abstractions.Results;

namespace Library.Application.Features.Borrowings.Commands;

public record BorrowBookCommand(Guid MemberId, Guid BookId) : ICommand<Result<BorrowingResponse>>;