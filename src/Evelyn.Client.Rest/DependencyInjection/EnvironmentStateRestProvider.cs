// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    using System;
    using Evelyn.Client.Provider;
    using Evelyn.Client.Rest;

    public static class EnvironmentStateRestProvider
    {
#pragma warning disable SA1614 // Element parameter documentation must have text
#pragma warning disable SA1616 // Element return value documentation must have text
        /// <summary>
        /// Use ReST to retrieve the environment state
        /// </summary>
        /// <param name="parentOptions"></param>
        /// <param name="actions"></param>
        public static void RestProvider(this EnvironmentStateProviderSetup parentOptions, Action<EnvironmentStateRestProviderOptions> actions)
#pragma warning restore SA1616 // Element return value documentation must have text
#pragma warning restore SA1614 // Element parameter documentation must have text
        {
            parentOptions.Services.AddSingleton<IEnvironmentStateProvider, Evelyn.Client.Rest.EnvironmentStateRestProvider>();
            parentOptions.Services.Configure(actions);
        }
    }
}
