namespace Evelyn.Core.WriteModel.Account.Events
{
    using System;

    public class ProjectCreated : Event
    {
        public ProjectCreated(string userId, Guid accountId, Guid projectId)
            : base(userId, accountId)
        {
            ProjectId = projectId;
        }

        public Guid ProjectId { get; set; }
    }
}
