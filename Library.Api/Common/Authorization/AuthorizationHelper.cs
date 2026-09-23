using System.Security.Claims;
using OpenIddict.Abstractions;

namespace Library.Api.Common.Authorization;

public static class AuthorizationHelper
{
    public static bool IsAdmin(ClaimsPrincipal user) =>
        user.FindFirst(OpenIddictConstants.Claims.Role)?.Value == "Admin";

    public static Guid? GetMemberId(ClaimsPrincipal user)
    {
        var value = user.FindFirst("member_id")?.Value;
        return value is not null ? Guid.Parse(value) : null;
    }

    public static bool IsOwnerOrAdmin(ClaimsPrincipal user, Guid targetMemberId) =>
        IsAdmin(user) || GetMemberId(user) == targetMemberId;
}