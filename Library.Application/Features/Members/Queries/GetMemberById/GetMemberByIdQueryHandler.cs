using Library.Application.Abstractions.Messaging;
using Library.Application.Abstractions.Repositories;
using Library.Domain.Exceptions;

namespace Library.Application.Features.Members.Queries.GetMemberById;

public class GetMemberByIdQueryHandler(IMemberRepository memberRepository)
    : IQueryHandler<GetMemberByIdQuery, MemberResponse>
{
    public async Task<MemberResponse> Handle(GetMemberByIdQuery request, CancellationToken cancellationToken)
    {
        var member = await memberRepository.GetByIdAsync(request.Id);
        if (member is null)
        {
            throw new NotFoundException("Member not found.");
        }

        return new MemberResponse(member.Id, member.FullName, member.Email, member.PhoneNumber, member.RegisteredDate, member.IsActive);
    }
}