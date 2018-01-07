namespace Evelyn.Core.ReadModel.Events
{
    using System;

    public class EnvironmentAdded : ApplicationEvent
    {
        public EnvironmentAdded(Guid applicationId, Guid environmentId, string name)
            : base(applicationId)
        {
            EnvironmentId = environmentId;
            Name = name;
        }

        public Guid EnvironmentId { get; set; }

        public string Name { get; set; }
    }
}
