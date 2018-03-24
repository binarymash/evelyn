namespace Evelyn.Core.WriteModel.Project.Events
{
    using System;

    public class ToggleStateDeleted : Event
    {
        public ToggleStateDeleted(string userId, Guid projectId, string environmentKey, string toggleKey, DateTimeOffset occurredAt)
            : base(userId, projectId, occurredAt)
        {
            EnvironmentKey = environmentKey;
            ToggleKey = toggleKey;
        }

        public string EnvironmentKey { get; set; }

        public string ToggleKey { get; set; }
    }
}
