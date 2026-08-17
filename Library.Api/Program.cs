using FluentValidation;
using Library.Api.Application.Interfaces;
using Library.Api.Application.Services;
using Library.Api.Endpoints;
using Library.Api.Infrastructure.Repositories.Implementations;
using Library.Api.Middleware;
using Library.Api.Validators;
using Library.Application.Abstractions.Repositories;
using Library.Application.Features.Books.Commands.CreateBook;
using Library.Infrastructure.Data;
//using Library.Infrastructure.Repositories;
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

// Application Services (still needed for un-converted endpoints)
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IBorrowingService, BorrowingService>();
builder.Services.AddScoped<IMemberService, MemberService>();

// MediatR — scans Library.Application for all commands/queries/handlers
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateBookCommand).Assembly));

// Validators — old ones still in Api, new ones now in Application
builder.Services.AddValidatorsFromAssemblyContaining<CreateBookCommandValidator>(); // Application assembly
builder.Services.AddValidatorsFromAssemblyContaining<UpdateBookRequestValidator>(); // old Api-assembly validators
builder.Services.AddValidatorsFromAssemblyContaining<CreateMemberRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateMemberRequestValidator>();
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