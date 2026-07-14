using Library.Api.Application.Interfaces;
using Library.Api.Application.Services;
using Library.Api.Infrastructure.Data;
using Library.Api.Infrastructure.Repositories.Implementations;
using Library.Api.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Library.Api.Endpoints;
using Library.Api.Middleware;
using Library.Api.Validators;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenApi();

builder.Services.AddDbContext<LibraryDbContext>(options =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("LibraryDb"));
});


// Repositories
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IBorrowingRepository, BorrowingRepository>();
builder.Services.AddScoped<IMemberRepository, MemberRepository>();

// Application Services
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IBorrowingService, BorrowingService>();
builder.Services.AddScoped<IMemberService, MemberService>();

// Validators
builder.Services.AddValidatorsFromAssemblyContaining<CreateBookRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateBookRequestValidator>();
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
// Endpoints
app.MapBookEndpoints();
app.MapMemberEndpoints();
app.MapBorrowingEndpoints();

app.Run();