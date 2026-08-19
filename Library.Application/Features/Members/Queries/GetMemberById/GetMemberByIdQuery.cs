using Library.Application.Abstractions.Messaging;

namespace Library.Application.Features.Members.Queries.GetMemberById;

public record GetMemberByIdQuery(Guid Id) : IQuery<MemberResponse>;