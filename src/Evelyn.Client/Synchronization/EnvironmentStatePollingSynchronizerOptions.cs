namespace Evelyn.Client.Synchronization
{
    using System;

    public class EnvironmentStatePollingSynchronizerOptions
    {
        public EnvironmentStatePollingSynchronizerOptions()
        {
            PollingPeriod = TimeSpan.FromSeconds(1);
        }

        /// <summary>
        /// Configures how often we will poll for the current environment state
        /// </summary>
        /// <value>How often we will poll for the current environment state</value>
        public TimeSpan PollingPeriod { get; set; }
    }
}
