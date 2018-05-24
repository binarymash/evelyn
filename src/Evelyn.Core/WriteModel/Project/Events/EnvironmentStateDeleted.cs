namespace Evelyn.Core.WriteModel.Project.Events
{
    using System;

    public class EnvironmentStateDeleted : Event
    {
        public EnvironmentStateDeleted(string userId, Guid projectId, string environmentKey, DateTimeOffset occurredAt)
            : base(userId, projectId, occurredAt)
        {
            EnvironmentKey = environmentKey;
        }

        public string EnvironmentKey { get; set; }
    }
}
