

namespace Library.Api.Application.contracts.Members;

public record CreateMemberRequest(
    string FullName,
    string Email,
    string PhoneNumber);