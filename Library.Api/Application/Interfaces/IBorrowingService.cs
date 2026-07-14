using Library.Api.Contracts.Borrowings;

namespace Library.Api.Application.Interfaces;

public interface IBorrowingService
{
    Task<BorrowingResponse> BorrowAsync(
        BorrowBookRequest request);

    Task<List<BorrowingResponse>> GetAllAsync();

    Task<List<BorrowingResponse>> Get(Guid memberId);

    Task ReturnAsync(Guid borrowingId);
}
