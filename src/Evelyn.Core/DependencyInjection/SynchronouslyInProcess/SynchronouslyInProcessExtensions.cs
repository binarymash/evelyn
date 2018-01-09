// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    using Evelyn.Core.ReadModel.ApplicationDetails;
    using Evelyn.Core.ReadModel.ApplicationList;
    using Evelyn.Core.ReadModel.EnvironmentDetails;
    using Evelyn.Core.ReadModel.ToggleDetails;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Options;

    public static class SynchronouslyInProcessExtensions
    {
#pragma warning disable SA1614 // Element parameter documentation must have text
                              /// <summary>
                              /// Publishes events synchronously and in-process.
                              /// All events will be processed before a response is returned to the user.
                              /// </summary>
                              /// <param name="parentRegistration"></param>
        public static void SynchronouslyInProcess(this EventPublisherRegistration parentRegistration)
#pragma warning restore SA1614 // Element parameter documentation must have text
        {
            parentRegistration.Services.TryAddSingleton<ApplicationDetailsHandler>();
            parentRegistration.Services.TryAddSingleton<ApplicationListHandler>();
            parentRegistration.Services.TryAddSingleton<EnvironmentDetailsHandler>();
            parentRegistration.Services.TryAddSingleton<ToggleDetailsHandler>();

            parentRegistration.Services.TryAddSingleton<IConfigureOptions<HandlerOptions>, ConfigureInProcessHandlerOptions>();
        }
    }
}
