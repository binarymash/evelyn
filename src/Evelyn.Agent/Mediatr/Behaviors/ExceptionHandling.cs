namespace Evelyn.Agent.Mediatr
{
    using System;
    using System.Threading.Tasks;
    using BinaryMash.Responses;
    using MediatR;
    using Microsoft.Extensions.Logging;

    public class ExceptionHandling<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TResponse : class
    {
        private ILogger logger;

        public ExceptionHandling(ILogger<Logging<TRequest, TResponse>> logger)
        {
            this.logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next)
        {
            try
            {
                return await next().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                logger.LogError(new EventId(0), ex, $"An internal exception occurred");
            }

            return BuildResponse
                .WithType<TResponse>()
                .AndWithErrors(new Error("InternalError", "An internal error occurred"))
                .Create();
        }
    }
}
