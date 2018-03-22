namespace Evelyn.Core.WriteModel.Account.Events
{
    using System;

    public class ProjectCreated : Event
    {
        public ProjectCreated(string userId, Guid accountId, Guid projectId, DateTimeOffset occurredAt)
            : base(userId, accountId, occurredAt)
        {
            ProjectId = projectId;
        }

        public Guid ProjectId { get; set; }
    }
}
