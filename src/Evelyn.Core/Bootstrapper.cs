namespace Evelyn.Core
{
    using System;
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
            projectionBuilderRegistrar.Register(typeof(EventStreamHandler), _handlerOptions.Value.ProjectionBuilders);

            startUpCommands.Execute().GetAwaiter().GetResult();
        }
    }
}
