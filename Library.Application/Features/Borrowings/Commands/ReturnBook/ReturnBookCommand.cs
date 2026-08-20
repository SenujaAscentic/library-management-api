using Library.Application.Abstractions.Messaging;
using Library.Application.Abstractions.Results;
using System.Windows.Input;

namespace Library.Application.Features.Borrowings.Commands.ReturnBook
{
    public record ReturnBookCommand(Guid BorrowingId): ICommand<Result<BorrowingResponse>>
    {

    }
}
