using Library.Application.Abstractions.Messaging;
using Library.Application.Abstractions.Repositories;
using Library.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Library.Application.Features.Members.Commands.DeleteMember;

public class DeleteMemberCommandHandler(IMemberRepository memberRepository, ILogger<DeleteMemberCommandHandler> logger)
    : ICommandHandler<DeleteMemberCommand>
{
    public async Task Handle(DeleteMemberCommand request, CancellationToken cancellationToken)
    {
        var member = await memberRepository.GetByIdAsync(request.Id);
        if (member is null)
        {
            logger.LogWarning("Delete rejected: member {MemberId} not found", request.Id);
            throw new NotFoundException("Member not found.");
        }

        memberRepository.Delete(member);
        await memberRepository.SaveChangesAsync();

        logger.LogInformation("Member deleted: {MemberId}", member.Id);
    }
}