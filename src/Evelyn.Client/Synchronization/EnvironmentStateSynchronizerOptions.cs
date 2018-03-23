namespace Evelyn.Client.Synchronization
{
    using System;

    public class EnvironmentStateSynchronizerOptions
    {
        public EnvironmentStateSynchronizerOptions()
        {
            SynchronizationPeriod = TimeSpan.FromSeconds(1);
        }

        public Guid ProjectId { get; set; }

        public string Environment { get; set; }

        public TimeSpan SynchronizationPeriod { get; set; }
    }
}
