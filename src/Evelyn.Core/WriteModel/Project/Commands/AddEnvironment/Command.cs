namespace Evelyn.Core.WriteModel.Project.Commands.AddEnvironment
{
    using System;

    public class Command : WriteModel.Command
    {
        public Command(string userId, Guid projectId, string key, int expectedProjectVersion)
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
