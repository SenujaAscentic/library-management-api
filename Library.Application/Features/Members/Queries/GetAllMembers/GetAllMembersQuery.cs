using Library.Application.Abstractions.Messaging;

namespace Library.Application.Features.Members.Queries.GetAllMembers;

public record GetAllMembersQuery : IQuery<List<MemberResponse>>;