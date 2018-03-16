// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    using Evelyn.Core.ReadModel;
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
            parentOptions.Services.TryAddSingleton<Evelyn.Core.ReadModel.AccountProjects.Handler>();
            parentOptions.Services.TryAddSingleton<
                IProjectionBuilder<Evelyn.Core.ReadModel.AccountProjects.ProjectionBuilderRequest, Evelyn.Core.ReadModel.AccountProjects.AccountProjectsDto>, 
                Evelyn.Core.ReadModel.AccountProjects.ProjectionBuilder>();

            parentOptions.Services.TryAddSingleton<Evelyn.Core.ReadModel.ProjectDetails.Handler>();
            parentOptions.Services.TryAddSingleton<
                IProjectionBuilder<Evelyn.Core.ReadModel.ProjectDetails.ProjectionBuilderRequest, Evelyn.Core.ReadModel.ProjectDetails.ProjectDetailsDto>,
                Evelyn.Core.ReadModel.ProjectDetails.ProjectionBuilder>();

            parentOptions.Services.TryAddSingleton<Evelyn.Core.ReadModel.EnvironmentDetails.Handler>();
            parentOptions.Services.TryAddSingleton<
                IProjectionBuilder<Evelyn.Core.ReadModel.EnvironmentDetails.ProjectionBuilderRequest, Evelyn.Core.ReadModel.EnvironmentDetails.EnvironmentDetailsDto>,
                Evelyn.Core.ReadModel.EnvironmentDetails.ProjectionBuilder>();

            parentOptions.Services.TryAddSingleton<Evelyn.Core.ReadModel.ToggleDetails.Handler>();
            parentOptions.Services.TryAddSingleton<
                IProjectionBuilder<Evelyn.Core.ReadModel.ToggleDetails.ProjectionBuilderRequest, Evelyn.Core.ReadModel.ToggleDetails.ToggleDetailsDto>,
                Evelyn.Core.ReadModel.ToggleDetails.ProjectionBuilder>();

            parentOptions.Services.TryAddSingleton<Evelyn.Core.ReadModel.EnvironmentState.Handler>();
            parentOptions.Services.TryAddSingleton<
                IProjectionBuilder<Evelyn.Core.ReadModel.EnvironmentState.ProjectionBuilderRequest, Evelyn.Core.ReadModel.EnvironmentState.EnvironmentStateDto>,
                Evelyn.Core.ReadModel.EnvironmentState.ProjectionBuilder>();

            parentOptions.Services.TryAddSingleton<IConfigureOptions<HandlerOptions>, ConfigureInProcessHandlerOptions>();
        }
    }
}
