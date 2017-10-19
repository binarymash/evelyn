namespace Evelyn.Core.ReadModel.Events
{
    using System;

    public class ToggleFlipped : ApplicationEvent
    {
        public Guid EnvironmentId { get; set; }

        public Guid ToggleId { get; set; }

        public bool Value { get; set; }

        public ToggleFlipped(Guid id, Guid environmentId, Guid toggleId, bool value) 
            : base(id)
        {
            EnvironmentId = environmentId;
            ToggleId = toggleId;
            Value = value;
        }
    }
}
