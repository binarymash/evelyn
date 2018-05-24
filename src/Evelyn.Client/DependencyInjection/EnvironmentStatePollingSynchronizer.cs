// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    using System;
    using Evelyn.Client.DependencyInjection;
    using Evelyn.Client.Synchronization;
    using Hosting;

    public static class EnvironmentStatePollingSynchronizer
    {
#pragma warning disable SA1614 // Element parameter documentation must have text
#pragma warning disable SA1616 // Element return value documentation must have text
        /// <summary>
        /// Use polling to synchronize the environment state
        /// </summary>
        /// <param name="parentOptions"></param>
        /// <param name="actions"></param>
        public static void Polling(this EnvironmentStateSynchronizerSetup parentOptions, Action<EnvironmentStatePollingSynchronizerSetup> actions)
#pragma warning restore SA1616 // Element return value documentation must have text
#pragma warning restore SA1614 // Element parameter documentation must have text
        {
            parentOptions.Services.AddSingleton<IHostedService, Evelyn.Client.Synchronization.EnvironmentStatePollingSynchronizer>();

            var setup = new EnvironmentStatePollingSynchronizerSetup(parentOptions.Services);
            actions.Invoke(setup);

            parentOptions.Services.Configure<EnvironmentStatePollingSynchronizerOptions>(options =>
            {
                options.PollingPeriod = setup.PollingPeriod;
            });
        }
    }
}
