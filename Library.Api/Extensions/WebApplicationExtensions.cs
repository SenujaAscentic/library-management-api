using Library.Domain.Entities;
using Library.Domain.Enums;
using Library.Infrastructure.Data;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;
using System.Text.Json;

namespace Library.Api.Extensions;

public static class WebApplicationExtensions
{
    public static async Task ApplyMigrationsAndSeedDataAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();
        await dbContext.Database.MigrateAsync();

        var appManager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();
        if (await appManager.FindByClientIdAsync("library-client") is null)
        {
            await appManager.CreateAsync(new OpenIddictApplicationDescriptor
            {
                ClientId = "library-client",
                ClientType = OpenIddictConstants.ClientTypes.Public,
                RedirectUris = { new Uri("https://localhost:7282/swagger/oauth2-redirect.html") },
                Permissions =
                {
                    OpenIddictConstants.Permissions.Endpoints.Authorization,
                    OpenIddictConstants.Permissions.Endpoints.Token,
                    OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                    OpenIddictConstants.Permissions.ResponseTypes.Code,
                    OpenIddictConstants.Permissions.Scopes.Profile,
                    OpenIddictConstants.Permissions.Prefixes.Scope + "library_api"
                },
                Requirements = { OpenIddictConstants.Requirements.Features.ProofKeyForCodeExchange }
            });
        }

        if (!await dbContext.Users.AnyAsync(u => u.Role == UserRole.Admin))
        {
            var hasher = new PasswordHasher<User>();
            var passwordHash = hasher.HashPassword(null!, "ChangeMe123!");
            var admin = User.Create("admin@library.local", passwordHash, UserRole.Admin, null);
            dbContext.Users.Add(admin);
            await dbContext.SaveChangesAsync();
        }
    }

    public static void MapHealthCheckEndpoints(this WebApplication app)
    {
        app.MapGet("/alive", () => Results.Ok(new { status = "alive" }));

        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = async (context, report) =>
            {
                context.Response.ContentType = "application/json";

                var payload = new
                {
                    status = report.Status.ToString(),
                    checks = report.Entries.Select(entry => new
                    {
                        name = entry.Key,
                        status = entry.Value.Status.ToString(),
                        description = entry.Value.Description
                    }),
                    totalDurationMs = report.TotalDuration.TotalMilliseconds
                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
            }
        });
    }
}