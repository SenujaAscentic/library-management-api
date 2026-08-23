using Library.Application.Abstractions;
using Library.Application.Abstractions.Messaging;
using Library.Application.Abstractions.Repositories;
using Library.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Library.Application.Features.Members.Commands.UpdateMember;

public class UpdateMemberCommandHandler(IMemberRepository memberRepository,IUnitOfWork unitOfWork, ILogger<UpdateMemberCommandHandler> logger)
    : ICommandHandler<UpdateMemberCommand, MemberResponse>
{
    public async Task<MemberResponse> Handle(UpdateMemberCommand request, CancellationToken cancellationToken)
    {
        var member = await memberRepository.GetByIdAsync(request.Id);
        if (member is null)
        {
            logger.LogWarning("Update rejected: member {MemberId} not found", request.Id);
            throw new NotFoundException("member_not_found","Member not found.");
        }

        var duplicateMember = await memberRepository.GetByEmailAsync(request.Email);
        if (duplicateMember is not null && duplicateMember.Id != request.Id)
        {
            logger.LogWarning("Update rejected: email {Email} already belongs to another member", request.Email);
            throw new ConflictException("duplicate_email","Email already exists.");
        }

        member.UpdateDetails(request.FullName, request.Email, request.PhoneNumber);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Member updated: {MemberId}", member.Id);

        return new MemberResponse(member.Id, member.FullName, member.Email, member.PhoneNumber, member.RegisteredDate, member.IsActive);
    }
}