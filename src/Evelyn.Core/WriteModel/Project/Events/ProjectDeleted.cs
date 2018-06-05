namespace Evelyn.Core.WriteModel.Project.Events
{
    using System;

    public class ProjectDeleted : Event
    {
        public ProjectDeleted(string userId, Guid projectId, DateTimeOffset occurredAt)
            : base(userId, projectId, occurredAt)
        {
        }
    }
}
