using Library.Domain.Entities;

namespace Library.Application.Abstractions.Repositories;
public interface IBorrowingRepository
{
    Task<List<Borrowing>> GetAllAsync();
    Task<Borrowing?> GetByIdAsync(Guid id);

    Task<List<Borrowing>> GetByMemberIdAsync(Guid memberId);

    Task<List<Borrowing>> GetActiveBorrowingsByMemberAsync(Guid memberId);
    Task<bool> HasActiveBorrowingForBookAsync(Guid bookId);
    Task AddAsync(Borrowing borrowing);
    void Update(Borrowing borrowing);
    void Delete(Borrowing borrowing);
    

}