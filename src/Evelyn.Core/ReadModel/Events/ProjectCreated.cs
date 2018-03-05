namespace Evelyn.Core.ReadModel.Events
{
    using System;

    public class ProjectCreated : ProjectEvent
    {
        public ProjectCreated(string userId, string accountId, Guid projectId, string name)
            : base(userId, projectId)
        {
            AccountId = accountId;
            Name = name;
            Version = -1;
        }

        public string AccountId { get; set; }

        public string Name { get; set; }
    }
}
