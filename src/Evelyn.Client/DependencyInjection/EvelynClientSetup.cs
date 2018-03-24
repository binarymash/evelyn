// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    using System;
    using Evelyn.Client;

    public class EvelynClientSetup : EvelynComponentSetup
    {
        public EvelynClientSetup(IServiceCollection services)
            : base(services)
        {
            SynchronizeEnvironmentStateUsing = new EnvironmentStateSynchronizerSetup(services);
        }

        /// <summary>Configures project that we are retrieving toggles for</summary>
        /// <value>The project that we are retrieving toggles for</value>
        public Guid ProjectId { get; set; }

        /// <summary>Configures the environment that we are retrieving toggles for</summary>
        /// <value>The environment that we are retrieving toggles for</value>
        public string Environment { get; set; }

        /// <summary>
        /// Configures how the environment state should be synchronized
        /// </summary>
        /// <value>
        /// The configuration for synchronizing the environment state
        /// </value>
        public EnvironmentStateSynchronizerSetup SynchronizeEnvironmentStateUsing { get; }

        public void EnvironmentOptions(Action<EnvironmentOptions> action)
        {
            Services.Configure(action);
        }
    }
}
