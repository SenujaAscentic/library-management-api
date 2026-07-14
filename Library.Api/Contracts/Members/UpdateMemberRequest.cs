namespace Library.Api.Application.contracts.Members;

public record UpdateMemberRequest(
    string FullName,
    string Email,
    string PhoneNumber);