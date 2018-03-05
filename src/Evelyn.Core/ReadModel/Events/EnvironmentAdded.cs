namespace Evelyn.Core.ReadModel.Events
{
    using System;

    public class EnvironmentAdded : ProjectEvent
    {
        public EnvironmentAdded(string userId, Guid projectId, Guid environmentId, string name)
            : base(userId, projectId)
        {
            EnvironmentId = environmentId;
            Name = name;
        }

        public Guid EnvironmentId { get; set; }

        public string Name { get; set; }
    }
}
