namespace Evelyn.Core
{
    using System;
    using System.Linq;
    using CQRSlite.Routing;
    using Evelyn.Core.ReadModel;
    using Evelyn.Core.ReadModel.Projections;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using WriteModel;

    public class Bootstrapper : IBootstrapper
    {
        private readonly IOptions<EventStreamHandlerOptions> _handlerOptions;

        public Bootstrapper(IOptions<EventStreamHandlerOptions> handlerOptions)
        {
            _handlerOptions = handlerOptions;
        }

        public void Bootstrap(IServiceProvider serviceProvider, IStartUpCommands startUpCommands)
        {
            var projectionBuilderRegistrar = serviceProvider.GetService<IProjectionBuilderRegistrar>();

            var projectionBuilders = _handlerOptions.Value.ProjectionBuilders.Select(pb => serviceProvider.GetService(pb) as IProjectionBuilder);
            projectionBuilderRegistrar.Register(typeof(EventStreamHandler), projectionBuilders);

            startUpCommands.Execute().GetAwaiter().GetResult();
        }
    }
}
