namespace Evelyn.Core.WriteModel.Project.Events
{
    using System;

    public class ToggleStateChanged : Event
    {
        public ToggleStateChanged(string userId, Guid projectId, string environmentKey, string toggleKey, string value, DateTimeOffset occurredAt)
            : base(userId, projectId, occurredAt)
        {
            EnvironmentKey = environmentKey;
            ToggleKey = toggleKey;
            Value = value;
        }

        public string EnvironmentKey { get; set; }

        public string ToggleKey { get; set; }

        public string Value { get; set; }
    }
}
