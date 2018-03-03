namespace Evelyn.Core.ReadModel.Events
{
    using System;

    public class EnvironmentAdded : ApplicationEvent
    {
        public EnvironmentAdded(string userId, Guid applicationId, Guid environmentId, string name)
            : base(userId, applicationId)
        {
            EnvironmentId = environmentId;
            Name = name;
        }

        public Guid EnvironmentId { get; set; }

        public string Name { get; set; }
    }
}
