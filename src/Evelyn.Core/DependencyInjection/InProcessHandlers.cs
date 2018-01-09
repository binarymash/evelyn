// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    using Evelyn.Core.ReadModel.ApplicationDetails;
    using Evelyn.Core.ReadModel.ApplicationList;
    using Evelyn.Core.ReadModel.EnvironmentDetails;
    using Evelyn.Core.ReadModel.ToggleDetails;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Options;

    public static class InProcessHandlers
    {
#pragma warning disable SA1614 // Element parameter documentation must have text
                              /// <summary>
                              /// Publishes events synchronously and in-process.
                              /// All events will be processed before a response is returned to the user.
                              /// </summary>
                              /// <param name="parentOptions"></param>
        public static void SynchronouslyInProcess(this EventPublisherOptions parentOptions)
#pragma warning restore SA1614 // Element parameter documentation must have text
        {
            parentOptions.Services.TryAddSingleton<ApplicationDetailsHandler>();
            parentOptions.Services.TryAddSingleton<ApplicationListHandler>();
            parentOptions.Services.TryAddSingleton<EnvironmentDetailsHandler>();
            parentOptions.Services.TryAddSingleton<ToggleDetailsHandler>();

            parentOptions.Services.TryAddSingleton<IConfigureOptions<HandlerOptions>, ConfigureInProcessHandlerOptions>();
        }
    }
}
