namespace Evelyn.Core.WriteModel.Project.Events
{
    using System;

    public class EnvironmentDeleted : Event
    {
        public EnvironmentDeleted(string userId, Guid projectId, string key, DateTimeOffset occurredAt)
            : base(userId, projectId, occurredAt)
        {
            Key = key;
        }

        public string Key { get; set; }
    }
}
