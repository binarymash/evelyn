namespace Evelyn.Client.DependencyInjection
{
    using System;
    using Microsoft.Extensions.DependencyInjection;

    public class EnvironmentStatePollingSynchronizerSetup : EvelynComponentSetup
    {
        public EnvironmentStatePollingSynchronizerSetup(IServiceCollection services)
            : base(services)
        {
            RetrieveEnvironmentStateUsing = new EnvironmentStateProviderSetup(services);
            PollingPeriod = TimeSpan.FromSeconds(1);
        }

        /// <summary>
        /// Configures how often we will poll for the current environment state
        /// </summary>
        /// <value>How often we will poll for the current environment state</value>
        public TimeSpan PollingPeriod { get; set; }

        public EnvironmentStateProviderSetup RetrieveEnvironmentStateUsing { get; }
    }
}
