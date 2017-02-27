namespace Evelyn.Agent.Mediatr.Behaviors
{
    using System.Threading.Tasks;
    using BinaryMash.Responses;
    using MediatR;
    using Microsoft.Extensions.Logging;

    public class Logging<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest
        where TResponse : Response
    {
        private ILogger logger;

        public Logging(ILogger<Logging<TRequest, TResponse>> logger)
        {
            this.logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next)
        {
            logger.LogInformation($"Handling {typeof(TRequest).Name}");
            var response = await next();
            logger.LogInformation($"Handled {typeof(TResponse).Name}");
            return response;
        }
    }
}
