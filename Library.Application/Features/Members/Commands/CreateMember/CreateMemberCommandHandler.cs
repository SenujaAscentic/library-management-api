using Library.Application.Abstractions;
using Library.Application.Abstractions.Messaging;
using Library.Application.Abstractions.Repositories;
using Library.Domain.Entities;
using Library.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Library.Application.Features.Members.Commands.CreateMember;

public class CreateMemberCommandHandler(IMemberRepository memberRepository,IUnitOfWork unitOfWork,ILogger<CreateMemberCommandHandler> logger)
    : ICommandHandler<CreateMemberCommand, MemberResponse>
{
    public async Task<MemberResponse> Handle(CreateMemberCommand request, CancellationToken cancellationToken)
    {
        var existingMember = await memberRepository.GetByEmailAsync(request.Email);
        if (existingMember is not null)
        {
            logger.LogWarning("Create member rejected: email {Email} already exists", request.Email);
            throw new ConflictException("duplicate_email","Email already exists.");
        }

        var member = Member.Create(request.FullName, request.Email, request.PhoneNumber);

        await memberRepository.AddAsync(member);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Member created: {MemberId}", member.Id);

        return new MemberResponse(member.Id, member.FullName, member.Email, member.PhoneNumber, member.RegisteredDate, member.IsActive);
    }
}