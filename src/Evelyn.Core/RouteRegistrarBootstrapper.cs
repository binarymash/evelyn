namespace Evelyn.Core
{
    using System;
    using CQRSlite.Routing;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using WriteModel;

    public class RouteRegistrarBootstrapper : IRouteRegistrarBootstrapper
    {
        private readonly IOptions<HandlerOptions> _handlerOptions;

        public RouteRegistrarBootstrapper(IOptions<HandlerOptions> handlerOptions)
        {
            _handlerOptions = handlerOptions;
        }

        public void Bootstrap(IServiceProvider serviceProvider, IStartUpCommands startUpCommands = default(NullStartUpCommands))
        {
            var registrar = new RouteRegistrar(serviceProvider);
            registrar.RegisterHandlers(_handlerOptions.Value.Handlers.ToArray());
            startUpCommands.Execute().GetAwaiter().GetResult();
        }
    }
}
