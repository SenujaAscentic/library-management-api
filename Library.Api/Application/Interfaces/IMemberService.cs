using Library.Api.Application.contracts.Members;
using Library.Api.Contracts.Members;

namespace Library.Api.Application.Interfaces;

public interface IMemberService
{
    Task<List<MemberResponse>> GetAllAsync();
    Task<MemberResponse?> GetByIdAsync(Guid id);
    Task<MemberResponse> CreateAsync(CreateMemberRequest request);
    Task UpdateAsync(Guid id, UpdateMemberRequest request);
    Task DeleteAsync(Guid id);
}