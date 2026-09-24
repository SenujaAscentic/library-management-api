using FluentValidation;
using Library.Api.Common.Swagger;
using Library.Application.Abstractions;
using Library.Application.Abstractions.Repositories;
using Library.Application.Features.Books.Commands.CreateBook;
using Library.Infrastructure;
using Library.Infrastructure.Data;
using Library.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.OpenApi;
using OpenIddict.Validation.AspNetCore;


namespace Library.Api.Extensions;

public static class BuilderExtensions
{
    public static void AddPersistence(this WebApplicationBuilder builder)
    {
        builder.AddNpgsqlDbContext<LibraryDbContext>("libraryDb");

        builder.Services.AddScoped<IBookRepository, BookRepository>();
        builder.Services.AddScoped<IBorrowingRepository, BorrowingRepository>();
        builder.Services.AddScoped<IMemberRepository, MemberRepository>();
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
    }

    public static void AddApplicationLayer(this WebApplicationBuilder builder)
    {
        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateBookCommand).Assembly));
        builder.Services.AddValidatorsFromAssemblyContaining<CreateBookCommandValidator>();
    }

    public static void AddAuthenticationAndAuthorization(this WebApplicationBuilder builder)
    {
        

        builder.Services
            .AddAuthentication(options =>
            {
                options.DefaultScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
            })
            .AddCookie();

        builder.Services.AddOpenIddict()
            .AddCore(options =>
            {
                options.UseEntityFrameworkCore().UseDbContext<LibraryDbContext>();
            })
            .AddServer(options =>
            {
                options.SetAuthorizationEndpointUris("/connect/authorize")
                       .SetTokenEndpointUris("/connect/token");

                options.AllowAuthorizationCodeFlow()
                       .RequireProofKeyForCodeExchange();

                options.RegisterScopes("openid", "profile", "library_api");

                options.AddDevelopmentEncryptionCertificate()
                       .AllowRefreshTokenFlow()
                       .AddDevelopmentSigningCertificate();

                options.UseAspNetCore()
                       .EnableAuthorizationEndpointPassthrough()
                       .EnableTokenEndpointPassthrough();
            })
            .AddValidation(options =>
            {
                options.UseLocalServer();
                options.UseAspNetCore();
            });

        builder.Services.AddAuthorization();
    }

    public static void AddApiDocumentation(this WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddOpenApi();

        builder.Services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    AuthorizationCode = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri("/connect/authorize", UriKind.Relative),
                        TokenUrl = new Uri("/connect/token", UriKind.Relative),
                        Scopes = new Dictionary<string, string>
                        {
                            { "openid", "OpenID" },
                            { "profile", "Profile" },
                            { "library_api", "Library API access" }
                        }
                    }
                }
            });
            options.OperationFilter<AuthorizeCheckOperationFilter>();

            options.AddSecurityRequirement(document =>
            {
                var schemeRef = new OpenApiSecuritySchemeReference("oauth2", document);
                return new OpenApiSecurityRequirement
                {
                    [schemeRef] = new List<string> { "library_api" }
                };
            });
        });
    }
}