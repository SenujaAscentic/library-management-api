using Library.Application.Abstractions.Messaging;

namespace Library.Application.Features.Auth.Register;


public record RegisterCommand(
    String FullName,
    String Email,
    String PhoneNumber,
    String Password) : ICommand<Guid>;