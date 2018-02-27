namespace Evelyn.Core.ReadModel.Events
{
    using System;

    public class ToggleStateChanged : ApplicationEvent
    {
        public ToggleStateChanged(Guid id, Guid environmentId, Guid toggleId, string value)
            : base(id)
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
