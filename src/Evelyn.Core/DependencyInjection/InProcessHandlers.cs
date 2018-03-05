﻿// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    using Evelyn.Core.ReadModel.EnvironmentDetails;
    using Evelyn.Core.ReadModel.ProjectDetails;
    using Evelyn.Core.ReadModel.ProjectList;
    using Evelyn.Core.ReadModel.ToggleDetails;
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
            parentOptions.Services.TryAddSingleton<ProjectDetailsHandler>();
            parentOptions.Services.TryAddSingleton<ProjectListHandler>();
            parentOptions.Services.TryAddSingleton<EnvironmentDetailsHandler>();
            parentOptions.Services.TryAddSingleton<ToggleDetailsHandler>();

            parentOptions.Services.TryAddSingleton<IConfigureOptions<HandlerOptions>, ConfigureInProcessHandlerOptions>();
        }
    }
}
