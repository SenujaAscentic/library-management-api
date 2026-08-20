using Library.Application.Abstractions.Messaging;
using Library.Application.Abstractions.Repositories;

namespace Library.Application.Features.Borrowings.Queries.GetBorrowingsByMember;

public class GetBorrowingsByMemberQueryHandler(IBorrowingRepository borrowingRepository)
    : IQueryHandler<GetBorrowingsByMemberQuery, List<BorrowingResponse>>
{
    public async Task<List<BorrowingResponse>> Handle(GetBorrowingsByMemberQuery request, CancellationToken cancellationToken)
    {
        var borrowings = await borrowingRepository.GetByMemberIdAsync(request.MemberId);
        return borrowings.Select(x => new BorrowingResponse(
            x.Id, x.BookId, x.MemberId, x.BorrowedDate, x.DueDate, x.ReturnedDate, x.Status.ToString())).ToList();
    }
}