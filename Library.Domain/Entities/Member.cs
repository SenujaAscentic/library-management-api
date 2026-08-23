namespace Library.Domain.Entities;

using Library.Domain.Exceptions;

public sealed class Member : BaseEntity
{
    public string FullName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PhoneNumber { get; private set; } = string.Empty;
    public DateTime RegisteredDate { get; private set; }
    public bool IsActive { get; private set; } = true;

    private Member() { } // EF Core

    public static Member Create(string fullName, string email, string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new BusinessRuleException("invalid_full_name","Full name cannot be empty.");
        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            throw new BusinessRuleException("invalid_email","A valid email is required.");
        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new BusinessRuleException("invalid_phone_number","Phone number cannot be empty.");

        return new Member
        {
            Id = Guid.NewGuid(),
            FullName = fullName,
            Email = email,
            PhoneNumber = phoneNumber,
            RegisteredDate = DateTime.UtcNow,
            IsActive = true
        };
    }

    public void UpdateDetails(string fullName, string email, string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new BusinessRuleException("invalid_full_name","Full name cannot be empty.");
        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            throw new BusinessRuleException("invalid_email","A valid email is required.");
        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new BusinessRuleException("invalid_phone_number","Phone number cannot be empty.");

        FullName = fullName;
        Email = email;
        PhoneNumber = phoneNumber;
    }
}