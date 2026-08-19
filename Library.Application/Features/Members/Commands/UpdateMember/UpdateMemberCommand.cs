using Library.Application.Abstractions.Messaging;

namespace Library.Application.Features.Members.Commands.UpdateMember;

public record UpdateMemberCommand(Guid Id, string FullName, string Email, string PhoneNumber) : ICommand<MemberResponse>;