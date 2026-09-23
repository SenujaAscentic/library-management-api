
using Library.Application.Abstractions.Messaging;
using Library.Domain.Entities;

namespace Library.Application.Features.Auth.Login;

public record LoginCommand(string Email, string Password) : ICommand<User>;