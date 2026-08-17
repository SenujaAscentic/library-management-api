using Library.Domain.Entities;

namespace Library.Application.Abstractions;

public interface IMemberRepository
{
    Task<List<Member>> GetAllAsync();

    Task<Member?> GetByIdAsync(Guid id);
    Task<Member?> GetByEmailAsync(string email);
    Task AddAsync(Member member);
    void Update(Member member);
    void Delete(Member member);
    Task SaveChangesAsync();
}