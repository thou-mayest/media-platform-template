using MediatR;

namespace SharedKernal.Messaging.Commands
{   
    public interface ICommand : IRequest;

    public interface ICommand<out TResponse> : IRequest<TResponse>;
}