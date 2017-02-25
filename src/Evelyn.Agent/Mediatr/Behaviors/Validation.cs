namespace Evelyn.Agent.Mediatr
{
    using System.Threading.Tasks;
    using MediatR;

    public class Validation<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next)
        {
            return await next();
        }
    }
}
