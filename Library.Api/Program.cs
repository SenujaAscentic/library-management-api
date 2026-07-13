using Library.Api.Infrastructure.Data;
using Library.Api.Infrastructure.Repositories.Implementations;
using Library.Api.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<LibraryDbContext>(options =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("LibraryDb"));
});

builder.Services.AddOpenApi();

builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IBorrowingRepository, BorrowingRepository>();
builder.Services.AddScoped<IMemberRepository, MemberRepository>();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapOpenApi();

app.Run();