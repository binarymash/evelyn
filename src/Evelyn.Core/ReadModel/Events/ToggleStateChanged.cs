namespace Evelyn.Core.ReadModel.Events
{
    using System;

    public class ToggleStateChanged : ProjectEvent
    {
        public ToggleStateChanged(string userId, Guid projectId, Guid environmentId, Guid toggleId, string value)
            : base(userId, projectId)
        {
            EnvironmentId = environmentId;
            ToggleId = toggleId;
            Value = value;
        }

        public Guid EnvironmentId { get; set; }

        public Guid ToggleId { get; set; }

        public string Value { get; set; }
    }
}
