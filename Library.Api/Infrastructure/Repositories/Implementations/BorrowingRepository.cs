using Library.Api.Domain.Entities;
using Library.Api.Domain.Enums;
using Library.Api.Infrastructure.Data;
using Library.Api.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace Library.Api.Infrastructure.Repositories.Implementations;

public class BorrowingRepository : IBorrowingRepository
{
    private readonly LibraryDbContext _db;

    public BorrowingRepository(LibraryDbContext db)
    {
        _db = db;
    }
    public async Task<List<Borrowing>> GetAllAsync()
    {
        return await _db.Borrowings.ToListAsync();
    }
    public async Task<Borrowing?> GetByIdAsync(Guid id)
    {
        return await _db.Borrowings.FirstOrDefaultAsync(x => x.Id == id);
    }
    public async Task<List<Borrowing>> GetByMemberIdAsync(Guid memberId)
    {
        return await _db.Borrowings.Where(x => x.MemberId == memberId).ToListAsync();
    }
    public async Task<List<Borrowing>> GetActiveBorrowingsByMemberAsync(Guid memberId)
    {
        return await _db.Borrowings
            .Where(x => x.MemberId == memberId && x.Status == BorrowingStatus.Borrowed)
            .ToListAsync();
    }
    public async Task AddAsync(Borrowing borrowing)
    {
        await _db.Borrowings.AddAsync(borrowing);
    }
    public void Update(Borrowing borrowing)
    {
        _db.Borrowings.Update(borrowing);
    }

    public void Delete(Borrowing borrowing)
    {
        _db.Borrowings.Remove(borrowing);
    }

    public async Task SaveChangesAsync()
    {
        await _db.SaveChangesAsync();
    }



}