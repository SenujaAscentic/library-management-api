using Library.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Library.Api.Infrastructure.Data;
using Library.Api.Infrastructure.Repositories.Interfaces;

namespace Library.Api.Infrastructure.Repositories.Implementations;

public class MemberRepository : IMemberRepository
{
    private readonly LibraryDbContext _db;

    public MemberRepository(LibraryDbContext db)
    {
        _db = db;
    }

    public async Task<List<Member>> GetAllAsync()
    {
        return await _db.Members.ToListAsync();
    }

    public async Task<Member?> GetByIdAsync(Guid id)
    {
        return await _db.Members.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Member?> GetByEmailAsync(string email)
    {
        return await _db.Members.FirstOrDefaultAsync(x => x.Email == email);
    }

    public async Task AddAsync(Member member)
    {
        await _db.Members.AddAsync(member);
        
    }

    public void Update(Member member)
    {
        _db.Members.Update(member);
    }

    public void Delete(Member member)
    {
        _db.Members.Remove(member);
    }

    public async Task SaveChangesAsync()
    {
        await _db.SaveChangesAsync();
    }

  
}