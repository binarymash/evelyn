// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    using System;
    using Evelyn.Client.Synchronization;

    public class EvelynClientOptions : EvelynComponentOptions
    {
        public EvelynClientOptions(IServiceCollection services)
            : base(services)
        {
            RetrieveEnvironmentStateUsing = new EnvironmentStateProviderOptions(services);
        }

        /// <summary>
        /// Configures how the environment state should be retrieved
        /// </summary>
        /// <value>
        /// The configuration for retrieving the environment state
        /// </value>
        public EnvironmentStateProviderOptions RetrieveEnvironmentStateUsing { get; }

        /// <summary>
        /// Configures the options for synchronizing the environment state
        /// </summary>
        /// <param name="action">An Action&lt;EnvironmentStateSynchronizerOptions&gt; to configure the environment state synchronization</param>
        public void SynchronizationOptions(Action<EnvironmentStateSynchronizerOptions> action)
        {
            Services.Configure(action);
        }
    }
}
