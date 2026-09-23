
namespace Library.Domain.Entities;

using Library.Domain.Enums;
using Library.Domain.Exceptions;

public sealed class User : BaseEntity
{
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public UserRole Role { get; private set; }
    public Guid? MemberId { get; private set; }

    private User() { } 

    public static User Create(string email, string passwordHash, UserRole role, Guid? memberId)
    {
        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            throw new BusinessRuleException("invalid_email", "A valid email is required.");

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new BusinessRuleException("invalid_password_hash", "Password hash cannot be empty.");

        if (role == UserRole.Admin && memberId is not null)
            throw new BusinessRuleException("admin_cannot_have_member_link", "An Admin user cannot be linked to a Member record.");

        if (role == UserRole.Member && memberId is null)
            throw new BusinessRuleException("member_user_requires_member_link", "A Member-role user must be linked to a Member record.");

        return new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            PasswordHash = passwordHash,
            Role = role,
            MemberId = memberId
        };
    }
}