namespace Evelyn.Core.WriteModel.Account.Events
{
    using System;

    public class ProjectDeleted : Event
    {
        public ProjectDeleted(string userId, Guid accountId, Guid projectId, DateTimeOffset occurredAt)
            : base(userId, accountId, occurredAt)
        {
            ProjectId = projectId;
        }

        public Guid ProjectId { get; set; }
    }
}
