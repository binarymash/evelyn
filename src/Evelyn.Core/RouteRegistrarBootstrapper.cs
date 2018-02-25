namespace Evelyn.Core
{
    using System;
    using CQRSlite.Routing;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;

    public class RouteRegistrarBootstrapper : IRouteRegistrarBootstrapper
    {
        private readonly IOptions<HandlerOptions> _handlerOptions;

        public RouteRegistrarBootstrapper(IOptions<HandlerOptions> handlerOptions)
        {
            _handlerOptions = handlerOptions;
        }

        public void Bootstrap(IServiceProvider serviceProvider)
        {
            var registrar = new RouteRegistrar(serviceProvider);
            registrar.RegisterHandlers(_handlerOptions.Value.Handlers.ToArray());
        }
    }
}
