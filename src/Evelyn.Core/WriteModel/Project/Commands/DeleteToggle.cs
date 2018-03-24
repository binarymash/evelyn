namespace Evelyn.Core.WriteModel.Project.Commands
{
    using System;

    public class DeleteToggle : Command
    {
        public DeleteToggle(string userId, Guid projectId, string key, int expectedVersion)
            : base(userId)
        {
            Key = key;
            ProjectId = projectId;
            ExpectedVersion = expectedVersion;
        }

        public int ExpectedVersion { get; set; }

        public Guid ProjectId { get; set; }

        public string Key { get; set; }
    }
}
