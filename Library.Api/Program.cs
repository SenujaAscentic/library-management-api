using Library.Api.Endpoints;
using Library.Api.Extensions;
using Library.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddPersistence();
builder.AddApplicationLayer();
builder.AddAuthenticationAndAuthorization();
builder.AddApiDocumentation();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    await app.ApplyMigrationsAndSeedDataAsync();
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.OAuthClientId("library-client");
        options.OAuthUsePkce();
    });
}

app.UseMiddleware<ExceptionMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.MapAuthEndpoints();
app.MapAuthorizeEndpoints();
app.MapBookEndpoints();
app.MapMemberEndpoints();
app.MapBorrowingEndpoints();
app.MapHealthCheckEndpoints();

app.Run();