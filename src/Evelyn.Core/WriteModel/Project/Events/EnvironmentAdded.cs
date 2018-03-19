namespace Evelyn.Core.WriteModel.Project.Events
{
    using System;

    public class EnvironmentAdded : Event
    {
        public EnvironmentAdded(string userId, Guid projectId, string key, DateTimeOffset occurredAt)
            : base(userId, projectId, occurredAt)
        {
            Key = key;
        }

        public string Key { get; set; }
    }
}
