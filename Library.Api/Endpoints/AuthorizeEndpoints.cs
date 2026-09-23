
using Library.Application.Features.Auth.Login;
using MediatR;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Extensions;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using System.Security.Claims;
using System.Web;

namespace Library.Api.Endpoints;

public static class AuthorizeEndpoints
{
    public static void MapAuthorizeEndpoints(this WebApplication app)
    {
        app.MapGet("/connect/authorize", async (HttpContext context) =>
        {
            var result = await context.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (!result.Succeeded)
            {
                var returnUrl = HttpUtility.UrlEncode(context.Request.GetEncodedUrl());

                return Results.Content($"""
                    <html>
                    <body>
                        <h3>Library Login</h3>
                        <form method="post" action="/connect/login">
                            <input type="hidden" name="returnUrl" value="{returnUrl}" />
                            <input type="email" name="email" placeholder="Email" required /><br/>
                            <input type="password" name="password" placeholder="Password" required /><br/>
                            <button type="submit">Log in</button>
                        </form>
                    </body>
                    </html>
                    """, "text/html");
            }

            return Results.SignIn(result.Principal!, properties: null, authenticationScheme: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        });

        app.MapPost("/connect/login", async (HttpContext context, IMediator mediator) =>
        {
            var form = await context.Request.ReadFormAsync();
            var email = form["email"].ToString();
            var password = form["password"].ToString();
            var returnUrl = form["returnUrl"].ToString();

            try
            {
                var user = await mediator.Send(new LoginCommand(email, password));

                var claims = new List<Claim>
                {
                    new(OpenIddictConstants.Claims.Subject, user.Id.ToString()),
                    new(OpenIddictConstants.Claims.Email, user.Email),
                    new(OpenIddictConstants.Claims.Role, user.Role.ToString())
                };
                if (user.MemberId is not null)
                {
                    claims.Add(new Claim("member_id", user.MemberId.Value.ToString()));
                }

                foreach (var claim in claims)
                {
                    claim.SetDestinations(GetDestinations(claim));
                }

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

                return Results.Redirect(HttpUtility.UrlDecode(returnUrl));
            }
            catch (Exception)
            {
                return Results.Content("<h3>Invalid email or password.</h3>", "text/html");
            }
        });

        app.MapPost("/connect/token", async (HttpContext context) =>
        {
            var request = context.GetOpenIddictServerRequest()
                ?? throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

            if (request.IsAuthorizationCodeGrantType() || request.IsRefreshTokenGrantType())
            {
                var result = await context.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
                return Results.SignIn(result.Principal!, properties: null, authenticationScheme: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            }

            throw new NotImplementedException("The specified grant type is not supported.");
        });
    }

    private static IEnumerable<string> GetDestinations(Claim claim)
    {
        switch (claim.Type)
        {
            case OpenIddictConstants.Claims.Subject:
            case OpenIddictConstants.Claims.Email:
            case OpenIddictConstants.Claims.Role:
            case "member_id":
                yield return OpenIddictConstants.Destinations.AccessToken;
                yield break;

            default:
                yield break;
        }
    }
}