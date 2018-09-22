// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    using Evelyn.Core.ReadModel;
    using Hosting;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Options;

    public static class InProcessHandlers
    {
#pragma warning disable SA1614 // Element parameter documentation must have text
        /// <summary>
        /// We will publish events directly to the read model handlers sychronously and in-process.
        /// WARNING! This means that all events will be processed before a response is returned to the user.
        /// This is not an optimal way to publish the events, as the end user experience
        /// will be slower and more prone to internal infrastructure failure.
        /// </summary>
        /// <param name="parentOptions"></param>
        public static void SynchronouslyInProcess(this EventPublisherOptions parentOptions)
#pragma warning restore SA1614 // Element parameter documentation must have text
        {
            parentOptions.Services.TryAddSingleton<IProjectionBuilderRegistrar, ProjectionBuilderRegistrar>();

            parentOptions.Services.AddHostedService<EventStreamHandler>();
            parentOptions.Services.TryAddSingleton<IEventStreamFactory, EventStreamFactory>();

            parentOptions.Services.TryAddSingleton<IConfigureOptions<EventStreamHandlerOptions>, ConfigureEventStreamHandlerOptions>();
            parentOptions.Services.TryAddSingleton<Evelyn.Core.ReadModel.Projections.AccountProjects.ProjectionBuilder>();
            parentOptions.Services.TryAddSingleton<Evelyn.Core.ReadModel.Projections.EnvironmentDetails.ProjectionBuilder>();
            parentOptions.Services.TryAddSingleton<Evelyn.Core.ReadModel.Projections.EnvironmentState.ProjectionBuilder>();
            parentOptions.Services.TryAddSingleton<Evelyn.Core.ReadModel.Projections.ProjectDetails.ProjectionBuilder>();
            parentOptions.Services.TryAddSingleton<Evelyn.Core.ReadModel.Projections.ToggleDetails.ProjectionBuilder>();
        }
    }
}
