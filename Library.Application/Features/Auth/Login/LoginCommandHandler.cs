
using Library.Application.Abstractions.Messaging;
using Library.Application.Abstractions.Repositories;
using Library.Domain.Entities;
using Library.Domain.Exceptions;
using Microsoft.AspNetCore.Identity;

namespace Library.Application.Features.Auth.Login;

public class LoginCommandHandler(IUserRepository userRepository)
    : ICommandHandler<LoginCommand, User>
{
    public async Task<User> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByEmailAsync(request.Email);
        if (user is null)
        {
            throw new NotFoundException("invalid_credentials", "Invalid email or password.");
        }

        var hasher = new PasswordHasher<User>();
        var result = hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (result == PasswordVerificationResult.Failed)
        {
            throw new NotFoundException("invalid_credentials", "Invalid email or password.");
        }

        return user;
    }
}