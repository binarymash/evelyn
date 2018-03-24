namespace Evelyn.Client.Synchronization
{
    using System;

    public class EnvironmentStateSynchronizerOptions
    {
        public EnvironmentStateSynchronizerOptions()
        {
            PollingPeriod = TimeSpan.FromSeconds(1);
        }

        /// <summary>Configures project that we are retrieving toggles for</summary>
        /// <value>The project that we are retrieving toggles for</value>
        public Guid ProjectId { get; set; }

        /// <summary>Configures the environment that we are retrieving toggles for</summary>
        /// <value>The environment that we are retrieving toggles for</value>
        public string Environment { get; set; }

        /// <summary>
        /// Configures how often we will poll for the current environment state
        /// </summary>
        /// <value>How often we will poll for the current environment state</value>
        public TimeSpan PollingPeriod { get; set; }
    }
}
