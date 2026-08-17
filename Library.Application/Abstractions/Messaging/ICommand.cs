using MediatR;

namespace Library.Application.Abstractions.Messaging
{
    public interface IBaseCommand { }

    public interface ICommand : IRequest, IBaseCommand { }
    public interface ICommand<TResponse> : IRequest<TResponse>, IBaseCommand { }

    public interface ICommandHandler<TCommand> : IRequestHandler<TCommand>
        where TCommand : ICommand
    { }

    public interface ICommandHandler<TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
        where TCommand : ICommand<TResponse>
    { }
}
