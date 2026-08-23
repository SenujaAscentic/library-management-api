using Library.Application.Abstractions;
using Library.Application.Abstractions.Messaging;
using Library.Application.Abstractions.Repositories;
using Library.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Library.Application.Features.Members.Commands.DeleteMember;

public class DeleteMemberCommandHandler(IMemberRepository memberRepository,IBorrowingRepository borrowingRepository,IUnitOfWork unitOfWork, ILogger<DeleteMemberCommandHandler> logger)
    : ICommandHandler<DeleteMemberCommand>
{
    public async Task Handle(DeleteMemberCommand request, CancellationToken cancellationToken)
    {
        var member = await memberRepository.GetByIdAsync(request.Id);
        if (member is null)
        {
            logger.LogWarning("Delete rejected: member {MemberId} not found", request.Id);
            throw new NotFoundException("member_not_found","Member not found.");
        }
        var activeBorrowings = await borrowingRepository.GetActiveBorrowingsByMemberAsync(request.Id);
        if (activeBorrowings.Count > 0)
        {
            logger.LogWarning("Delete rejected: member {MemberId} has active borrowings.", request.Id);
            throw new BusinessRuleException("member_has_active_borrowings","Cannot delete a member with active borrowings.");
        }

        memberRepository.Delete(member);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Member deleted: {MemberId}", member.Id);
    }
}