using Library.Application.Abstractions.Messaging;
using Library.Application.Abstractions.Results;

namespace Library.Application.Features.Borrowings.Commands.ReturnBook
{
    public record ReturnBookCommand(Guid BorrowingId): ICommand<Result<BorrowingResponse>>
    {

    }
}
