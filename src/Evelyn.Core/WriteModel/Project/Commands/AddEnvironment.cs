namespace Evelyn.Core.WriteModel.Project.Commands
{
    using System;

    public class AddEnvironment : Command
    {
        public AddEnvironment(string userId, Guid projectId, string key, int expectedProjectVersion)
            : base(userId)
        {
            Key = key;
            ProjectId = projectId;
            ExpectedProjectVersion = expectedProjectVersion;
        }

        public int ExpectedProjectVersion { get; set; }

        public Guid ProjectId { get; set; }

        public string Key { get; set; }
    }
}
