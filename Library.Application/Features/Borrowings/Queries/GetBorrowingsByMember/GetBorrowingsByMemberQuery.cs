using Library.Application.Abstractions.Messaging;

namespace Library.Application.Features.Borrowings.Queries.GetBorrowingsByMember;

public record GetBorrowingsByMemberQuery(Guid MemberId) : IQuery<List<BorrowingResponse>>;