using Library.Application.Abstractions.Messaging;
using Library.Application.Abstractions.Repositories;

namespace Library.Application.Features.Members.Queries.GetAllMembers;

public class GetAllMembersQueryHandler(IMemberRepository memberRepository)
    : IQueryHandler<GetAllMembersQuery, List<MemberResponse>>
{
    public async Task<List<MemberResponse>> Handle(GetAllMembersQuery request, CancellationToken cancellationToken)
    {
        var members = await memberRepository.GetAllAsync();
        return members.Select(m => new MemberResponse(m.Id, m.FullName, m.Email, m.PhoneNumber, m.RegisteredDate, m.IsActive)).ToList();
    }
}