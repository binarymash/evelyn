namespace Evelyn.Core.ReadModel.Events
{
    using System;

    public class ToggleStateChanged : ProjectEvent
    {
        public ToggleStateChanged(string userId, Guid projectId, string environmentKey, Guid toggleId, string value)
            : base(userId, projectId)
        {
            EnvironmentKey = environmentKey;
            ToggleId = toggleId;
            Value = value;
        }

        public string EnvironmentKey { get; set; }

        public Guid ToggleId { get; set; }

        public string Value { get; set; }
    }
}
