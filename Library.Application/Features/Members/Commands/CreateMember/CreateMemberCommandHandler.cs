using Library.Application.Abstractions.Messaging;
using Library.Application.Abstractions.Repositories;
using Library.Domain.Entities;
using Library.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Library.Application.Features.Members.Commands.CreateMember;

public class CreateMemberCommandHandler(IMemberRepository memberRepository, ILogger<CreateMemberCommandHandler> logger)
    : ICommandHandler<CreateMemberCommand, MemberResponse>
{
    public async Task<MemberResponse> Handle(CreateMemberCommand request, CancellationToken cancellationToken)
    {
        var existingMember = await memberRepository.GetByEmailAsync(request.Email);
        if (existingMember is not null)
        {
            logger.LogWarning("Create member rejected: email {Email} already exists", request.Email);
            throw new ConflictException("Email already exists.");
        }

        var member = new Member
        {
            Id = Guid.NewGuid(),
            FullName = request.FullName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            RegisteredDate = DateTime.UtcNow,
            IsActive = true
        };

        await memberRepository.AddAsync(member);
        await memberRepository.SaveChangesAsync();

        logger.LogInformation("Member created: {MemberId}", member.Id);

        return new MemberResponse(member.Id, member.FullName, member.Email, member.PhoneNumber, member.RegisteredDate, member.IsActive);
    }
}