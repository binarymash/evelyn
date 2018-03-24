// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    using System;
    using Evelyn.Client.Provider;
    using Evelyn.Client.Rest;

    public static class EnvironmentStateRestProvider
    {
#pragma warning disable SA1614 // Element parameter documentation must have text
        /// <summary>
        /// Use ReST to retrieve the environment state
        /// </summary>
        /// <param name="parentOptions"></param>
        /// <param name="actions"></param>
        public static void RestProvider(this EnvironmentStateProviderOptions parentOptions, Action<EnvironmentStateRestProviderOptions> actions)
        {
            parentOptions.Services.AddSingleton<IEnvironmentStateProvider, Evelyn.Client.Rest.EnvironmentStateRestProvider>();
            parentOptions.Services.Configure(actions);
        }
    }
}
