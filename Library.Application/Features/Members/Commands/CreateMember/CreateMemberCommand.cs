using Library.Application.Abstractions.Messaging;

namespace Library.Application.Features.Members.Commands.CreateMember;

public record CreateMemberCommand(
    string FullName,
    string Email,
    string PhoneNumber):ICommand<MemberResponse>;