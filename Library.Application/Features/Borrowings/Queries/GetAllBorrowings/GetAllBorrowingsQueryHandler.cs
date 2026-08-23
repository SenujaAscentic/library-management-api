using Library.Application.Abstractions.Messaging;
using Library.Application.Abstractions.Repositories;

namespace Library.Application.Features.Borrowings.Queries.GetAllBorrowings;

public class GetAllBorrowingsQueryHandler(IBorrowingRepository borrowingRepository)
    : IQueryHandler<GetAllBorrowingsQuery, List<BorrowingResponse>>
{
    public async Task<List<BorrowingResponse>> Handle(GetAllBorrowingsQuery request, CancellationToken cancellationToken)
    {
        var borrowings = await borrowingRepository.GetAllAsync();
        return borrowings.Select(x => new BorrowingResponse(
            x.Id, x.BookId, x.MemberId, x.BorrowedDate, x.DueDate, x.ReturnedDate, x.Status.ToString())).ToList();
    }
}