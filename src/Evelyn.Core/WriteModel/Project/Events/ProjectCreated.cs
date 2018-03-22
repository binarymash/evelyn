namespace Evelyn.Core.WriteModel.Project.Events
{
    using System;

    public class ProjectCreated : Event
    {
        public ProjectCreated(string userId, Guid accountId, Guid projectId, string name, DateTimeOffset occurredAt)
            : base(userId, projectId, occurredAt)
        {
            AccountId = accountId;
            Name = name;
            Version = -1;
        }

        public Guid AccountId { get; set; }

        public string Name { get; set; }
    }
}
