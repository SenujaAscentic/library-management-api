using FluentValidation;
using Library.Api.Application.Interfaces;
using Library.Api.Application.Services;
using Library.Api.Endpoints;
using Library.Api.Middleware;
using Library.Api.Validators;
using Library.Application.Abstractions.Repositories;
using Library.Application.Features.Books.Commands.CreateBook;
using Library.Application.Features.Books.Commands.UpdateBook;
using Library.Infrastructure.Data;
using Library.Infrastructure.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenApi();

builder.Services.AddDbContext<LibraryDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("LibraryDb"));
});

// Repositories
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IBorrowingRepository, BorrowingRepository>();
builder.Services.AddScoped<IMemberRepository, MemberRepository>();

// Application Services (still needed for un-converted Member/Borrowing endpoints)
builder.Services.AddScoped<IBorrowingService, BorrowingService>();


// MediatR — scans Library.Application for all commands/queries/handlers
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateBookCommand).Assembly));

// Validators
builder.Services.AddValidatorsFromAssemblyContaining<CreateBookCommandValidator>(); // Application assembly (Books)
builder.Services.AddValidatorsFromAssemblyContaining<BorrowBookRequestValidator>();

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

app.Run();