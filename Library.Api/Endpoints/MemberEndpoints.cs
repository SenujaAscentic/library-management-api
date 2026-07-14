
using Library.Api.Application.contracts.Members;
using Library.Api.Application.Interfaces;

namespace Library.Api.Endpoints;

public static class MemberEndpoints
{
    public static void MapMemberEndpoints(this WebApplication app)
    {
        app.MapPost("/api/members", async (CreateMemberRequest request, IMemberService service)=>
        {
            var member = await service.CreateAsync(request);
            return Results.Created($"/api/members/{member.Id}",member);
        });
        app.MapGet("/api/members", async (IMemberService service) =>
        {
            var members = await service.GetAllAsync();
            return Results.Ok(members);
        });
        app.MapGet("/api/members/{id:guid}", async (Guid id, IMemberService service) =>
        {
            var member = await service.GetByIdAsync(id);
            return member is null ? Results.NotFound() : Results.Ok(member);
        });
        app.MapPut("/api/members/{id:guid}",async(Guid id, UpdateMemberRequest request, IMemberService service) =>
        {
            await service.UpdateAsync(id, request);
            return Results.NoContent(); 
        });
        app.MapDelete("/api/members/{id:guid}", async (Guid id, IMemberService service) =>
        {
            await service.DeleteAsync(id);
            return Results.NoContent();
        });

    }
}