namespace Evelyn.Core.WriteModel.Project.Commands
{
    using System;
    using Evelyn.Core.WriteModel;

    public class AddEnvironment : Command
    {
        public AddEnvironment(string userId, Guid projectId, string key, int? expectedVersion = null)
            : base(userId)
        {
            Key = key;
            ProjectId = projectId;
            ExpectedVersion = expectedVersion;
        }

        public int? ExpectedVersion { get; set; }

        public Guid ProjectId { get; set; }

        public string Key { get; set; }
    }
}
