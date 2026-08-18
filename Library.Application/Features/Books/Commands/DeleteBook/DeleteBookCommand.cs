using Library.Application.Abstractions.Messaging;

namespace Library.Application.Features.Books.Commands.DeleteBook;

    public record DeleteBookCommand(Guid Id) : ICommand;
    

