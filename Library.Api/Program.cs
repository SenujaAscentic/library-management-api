using Library.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<LibraryDbContext>(options =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("LibraryDb"));
});

builder.Services.AddOpenApi();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapOpenApi();

app.Run();