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
        
            // Swagger (browser) client
            await UpsertClientAsync(appManager, CreatePublicClient(
                clientId: "library-client",
                displayName: "Swagger UI",
                redirectUris: new[]
                {
                    "https://localhost:7282/swagger/oauth2-redirect.html",
                    "http://localhost:5173/swagger/oauth2-redirect.html"
                }));

            // Flutter mobile client. Must match flutter_appauth's redirectUrl EXACTLY.
            await UpsertClientAsync(appManager, CreatePublicClient(
                clientId: "library-mobile",
                displayName: "Library Mobile App",
                redirectUris: new[] { "com.example.librarymanagementapp:/oauthredirect" }));
        

        if (!await dbContext.Users.AnyAsync(u => u.Role == UserRole.Admin))
        {
            var hasher = new PasswordHasher<User>();
            var passwordHash = hasher.HashPassword(null!, "ChangeMe123!");
            var admin = User.Create("admin@library.local", passwordHash, UserRole.Admin, null);
            dbContext.Users.Add(admin);
            await dbContext.SaveChangesAsync();
        }
    }
    private static OpenIddictApplicationDescriptor CreatePublicClient(
    string clientId, string displayName, IEnumerable<string> redirectUris)
    {
        var descriptor = new OpenIddictApplicationDescriptor
        {
            ClientId = clientId,
            DisplayName = displayName,
            ClientType = OpenIddictConstants.ClientTypes.Public,
            Permissions =
        {
            OpenIddictConstants.Permissions.Endpoints.Authorization,
            OpenIddictConstants.Permissions.Endpoints.Token,
            OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
            OpenIddictConstants.Permissions.GrantTypes.RefreshToken,   // NEW
            OpenIddictConstants.Permissions.ResponseTypes.Code,
            OpenIddictConstants.Permissions.Scopes.Profile,
            OpenIddictConstants.Permissions.Prefixes.Scope + "library_api"
        },
            Requirements = { OpenIddictConstants.Requirements.Features.ProofKeyForCodeExchange }
        };

        foreach (var uri in redirectUris)
        {
            descriptor.RedirectUris.Add(new Uri(uri));
        }

        return descriptor;
    }

    // Create-or-update, so code changes actually reach an existing database.
    private static async Task UpsertClientAsync(
        IOpenIddictApplicationManager manager, OpenIddictApplicationDescriptor descriptor)
    {
        var existing = await manager.FindByClientIdAsync(descriptor.ClientId!);
        if (existing is null)
        {
            await manager.CreateAsync(descriptor);
        }
        else
        {
            await manager.UpdateAsync(existing, descriptor);
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