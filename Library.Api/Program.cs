using FluentValidation;
using Library.Api.Endpoints;
using Library.Api.Middleware;
using Library.Application.Abstractions;
using Library.Application.Abstractions.Repositories;
using Library.Application.Features.Books.Commands.CreateBook;
using Library.Infrastructure;
using Library.Infrastructure.Data;
using Library.Infrastructure.Repositories;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("LibraryDb")
    ?? throw new InvalidOperationException("Connection string 'LibraryDb' is missing from configuration.");

builder.Services.AddDbContext<LibraryDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});

// Repositories
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IBorrowingRepository, BorrowingRepository>();
builder.Services.AddScoped<IMemberRepository, MemberRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// MediatR — scans Library.Application for all commands/queries/handlers
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateBookCommand).Assembly));

// Validators — scans the whole Library.Application assembly (Books + Members; Borrowings has none by design)
builder.Services.AddValidatorsFromAssemblyContaining<CreateBookCommandValidator>();

// Health checks
builder.Services.AddHealthChecks()
    .AddNpgSql(connectionString, name: "postgresql");

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionMiddleware>();

app.MapBookEndpoints();
app.MapMemberEndpoints();
app.MapBorrowingEndpoints();

// Liveness — fast, no dependency checks, just "is the process up"
app.MapGet("/alive", () => Results.Ok(new { status = "alive" }));

// Readiness — checks dependencies (PostgreSQL) that matter for serving real traffic
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

app.Run();