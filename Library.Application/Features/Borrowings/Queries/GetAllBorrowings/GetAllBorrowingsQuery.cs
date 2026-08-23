using Library.Application.Abstractions.Messaging;

namespace Library.Application.Features.Borrowings.Queries.GetAllBorrowings;

public record GetAllBorrowingsQuery : IQuery<List<BorrowingResponse>>;