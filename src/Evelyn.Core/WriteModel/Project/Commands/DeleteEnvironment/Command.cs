namespace Evelyn.Core.WriteModel.Project.Commands.DeleteEnvironment
{
    using System;

    public class Command : WriteModel.Command
    {
        public Command(string userId, Guid projectId, string key, int expectedEnvironmentVersion)
            : base(userId)
        {
            Key = key;
            ProjectId = projectId;
            ExpectedEnvironmentVersion = expectedEnvironmentVersion;
        }

        public int ExpectedEnvironmentVersion { get; set; }

        public Guid ProjectId { get; set; }

        public string Key { get; set; }
    }
}
