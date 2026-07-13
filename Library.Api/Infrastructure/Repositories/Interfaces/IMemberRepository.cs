

using Library.Api.Domain.Entities;

public interface IMemberRepository
{
    Task<List<Member>> GetAllAsync();

    Task<Member?> GetByIdAsync(Guid id);
    Task<Member?> AddAsync(Member member);
    void Update(Member member);
    void Delete(Member member);
    Task SaveChangesAsync();
}