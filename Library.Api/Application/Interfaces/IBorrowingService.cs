using Library.Api.Contracts.Borrowings;

namespace Library.Api.Application.Interfaces;

public interface IBorrowingService
{
    

    Task<List<BorrowingResponse>> GetAllAsync();

    Task<List<BorrowingResponse>> GetByMemberAsync(Guid memberId);

    
}
