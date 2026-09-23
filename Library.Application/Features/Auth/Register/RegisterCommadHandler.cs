
using Library.Application.Abstractions;
using Library.Application.Abstractions.Messaging;
using Library.Application.Abstractions.Repositories;
using Library.Domain.Entities;
using Library.Domain.Enums;
using Library.Domain.Exceptions;
using Microsoft.AspNetCore.Identity;

namespace Library.Application.Features.Auth.Register;

public class RegisterCommandHandler(
    IMemberRepository memberRepository,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<RegisterCommand, Guid>
{
    public async Task<Guid> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await userRepository.GetByEmailAsync(request.Email);
        if (existingUser is not null)
        {
            throw new ConflictException("duplicate_email", "Email already exists.");
        }

        var existingMember = await memberRepository.GetByEmailAsync(request.Email);
        if (existingMember is not null)
        {
            throw new ConflictException("duplicate_email", "Email already exists.");
        }

        var member = Member.Create(request.FullName, request.Email, request.PhoneNumber);
        await memberRepository.AddAsync(member);

        var hasher = new PasswordHasher<User>();
        var passwordHash = hasher.HashPassword(null!, request.Password);
        var user = User.Create(request.Email, passwordHash, UserRole.Member, member.Id);
        await userRepository.AddAsync(user);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return member.Id;
    }
}