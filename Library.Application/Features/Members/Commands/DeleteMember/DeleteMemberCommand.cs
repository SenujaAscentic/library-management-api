using Library.Application.Abstractions.Messaging;

namespace Library.Application.Features.Members.Commands.DeleteMember;

public record DeleteMemberCommand(Guid Id) : ICommand;