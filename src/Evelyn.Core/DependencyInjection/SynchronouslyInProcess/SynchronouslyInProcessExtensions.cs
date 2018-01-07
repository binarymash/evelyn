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
        public static void SynchronouslyInProcess(this EventPublisherRegistration parentRegistration)
        {
            parentRegistration.Services.TryAddSingleton<ApplicationDetailsHandler>();
            parentRegistration.Services.TryAddSingleton<ApplicationListHandler>();
            parentRegistration.Services.TryAddSingleton<EnvironmentDetailsHandler>();
            parentRegistration.Services.TryAddSingleton<ToggleDetailsHandler>();

            parentRegistration.Services.TryAddSingleton<IConfigureOptions<HandlerOptions>, ConfigureInProcessHandlerOptions>();
        }
    }
}
