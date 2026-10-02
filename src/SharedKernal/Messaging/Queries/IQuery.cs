using MediatR;

namespace SharedKernal.Messaging.Queries
{
    public interface IQuery<out TResponse> : IRequest<TResponse>;
}